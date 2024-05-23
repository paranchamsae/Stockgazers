#-*- coding:utf-8 -*-
from fastapi import APIRouter, HTTPException, status, File, UploadFile
from fastapi.responses import JSONResponse, FileResponse
import math
from database import get_db
from datetime import datetime

from pandas import pandas       # excel export library
from io import BytesIO, StringIO

from sqlalchemy import select, text, update
from models import Stocks

from domain.stocks import stocks_schema

router = APIRouter(
    prefix = "/api/stocks",
    tags=["Stocks"]
)

@router.get("/{UserID}", summary="내 재고 현황 불러오기")
async def get_stocks(UserID: str):
    with get_db() as db:
        result = db.query(Stocks).filter(Stocks.UserID == int(UserID)).all()

    return result

@router.get("/export/{UserID}", summary="내 재고 현황 엑셀로 내려받기(구매원가 업로드용)")
async def get_stocks_excel(UserID: str):
    elements = []
    with get_db() as db:
        result = db.query(Stocks).filter(Stocks.UserID == int(UserID)).all()
        for row in result:
            testrow = []
            testrow.append(row.StyleId)
            testrow.append(row.Title)
            testrow.append(row.VariantValue)
            testrow.append(row.ListingID)
            testrow.append("")
            elements.append(testrow)

    frame = pandas.DataFrame(
        elements,
        columns = ["모델명", "이름", "사이즈", "고유아이디", "구매원가"]
    )

    filename = "data-"+UserID+"-"+datetime.now().strftime('%Y%m%d')+".csv"
    frame.to_csv(filename, index=False, encoding="utf-8-sig")

    return FileResponse(
        filename,
        filename=filename,
        media_type="text/csv"
    )

@router.post("/import", summary="내 재고 현황 엑셀 업로드(구매원가 업로드용)")
async def set_stocks_excel(csvfile: UploadFile = File(...)):
    dataframe = pandas.read_csv(csvfile.file, encoding_errors='ignore')
    csvfile.file.close()
    # print(dataframe)

    with get_db() as db:
        for row in dataframe.iterrows():
            query = text("select * from SGStocks where ListingID='"+row[1][3]+"' ")
            result = db.execute(query).mappings().all()

            if math.isnan(float(row[1][4])):
                continue            

            if result[0]["OrderNo"] == "":
                query = update(Stocks).where(Stocks.ListingID == row[1][3]).values(
                    BuyPrice = int(row[1][4]),
                    BuyPriceUSD = int(row[1][4])/1300
                )
            else:
                query = update(Stocks).where(Stocks.ListingID == row[1][3]).values(
                    BuyPrice = int(row[1][4]),
                    BuyPriceUSD = int(row[1][4])/1300,
                )
            
            try:
                db.execute(query)
                print(query)
            except Exception as e:
                db.rollback()
                print(e)
            finally:
                db.commit()
                print("")

    return JSONResponse(
        status_code=status.HTTP_200_OK,
        content={
            "message": "ok"
        }
    )

@router.post("", summary="", status_code=status.HTTP_201_CREATED)
async def addStocks(request: list[stocks_schema.RequestAddStocks]):
    with get_db() as db:
        for row in request:
            new_stock = Stocks(
                UserID = row.UserID,
                IsDelete = "F",
                ListingID = row.ListingID,
                StyleId = row.StyleID,
                ProductID = row.ProductID,
                Title = row.Title,
                VariantID = row.VariantID,
                VariantValue = row.VariantValue,
                BuyPrice = row.BuyPrice,
                BuyPriceUSD = row.BuyPriceUSD,
                Price = row.Price,
                Limit = row.Limit,
                OrderNo = row.OrderNo,
                SellDatetime = row.SellDatetime,
                SendDatetime = row.SendDatetime,
                AdjustPrice = row.AdjustPrice,
                Profit = row.Profit,
                CreateDatetime = datetime.now().strftime('%Y-%m-%d %H:%M:%S'),
                UpdateDatetime = None
            )
            db.add(new_stock)
        db.commit()

    return JSONResponse(
        status_code=status.HTTP_201_CREATED,
        content={
            "message": "ok"
        }
    )