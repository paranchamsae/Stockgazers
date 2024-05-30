#-*- coding:utf-8 -*-
from fastapi import APIRouter, HTTPException, status, File, UploadFile
from fastapi.responses import JSONResponse, FileResponse
from fastapi.encoders import jsonable_encoder
import math
from database import get_db
from datetime import datetime

from pandas import pandas       # excel export library
from io import BytesIO, StringIO

from sqlalchemy import select, text, update, and_
from models import Stocks

from domain.stocks import stocks_schema

router = APIRouter(
    prefix = "/api/stocks",
    tags=["Stocks"]
)

@router.get("/{UserID}", summary="내 재고 현황 불러오기")
async def get_stocks(UserID: str):
    with get_db() as db:
        result = db.query(Stocks).filter(and_(Stocks.UserID == int(UserID), Stocks.IsDelete == "F")).all()

    return result

@router.get("/{UserID}/{Status}")
async def get_stocks_conditions(UserID: int, Status: str):
    with get_db() as db:
        result = db.query(Stocks).filter(and_(Stocks.UserID == UserID, Stocks.IsDelete == "F", Stocks.Status == Status.upper())).all()
    
    return JSONResponse(
        status_code=status.HTTP_200_OK,
        content={
            "message": "ok",
            "data": jsonable_encoder(result)
        }
    )

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

            if result[0]["AdjustPrice"] > 0:
                tempProfit = ((int(row[1][4])/1300)-result[0]["AdjustPrice"])/(int(row[1][4])/1300)*100
                query = update(Stocks).where(Stocks.ListingID == row[1][3]).values(
                    BuyPrice = int(row[1][4]),
                    BuyPriceUSD = int(row[1][4])/1300,
                    Profit = round(tempProfit, 2),
                    UpdateDatetime = datetime.now().strftime('%Y-%m-%d %H:%M:%S')
                )
            else:
                query = update(Stocks).where(Stocks.ListingID == row[1][3]).values(
                    BuyPrice = int(row[1][4]),
                    BuyPriceUSD = int(row[1][4])/1300,
                    UpdateDatetime = datetime.now().strftime('%Y-%m-%d %H:%M:%S')
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
                UpdateDatetime = None,
                Status = row.Status
            )
            db.add(new_stock)
        db.commit()

    return JSONResponse(
        status_code=status.HTTP_201_CREATED,
        content={
            "message": "ok"
        }
    )

@router.patch("/order", summary="판매 완료 데이터에 판매정보 업데이트")
async def patchorder(request: list[stocks_schema.RequestPatchOrder]):
    with get_db() as db:
        for row in request:
            # 해당 레코드에 구매원가 데이터가 있다면 profit 데이터도 계산하여 업데이트 같이 쳐줌
            result = db.query(Stocks).filter(Stocks.ListingID == row.ListingID).all()
            if result[0]["BuyPrice"] > 0 and result[0]["BuyPriceUSD"] > 0:
                query = update(Stocks).where(Stocks.ListingID == row.ListingID).values(
                    OrderNo = row.OrderNo,
                    AdjustPrice = row.AdjustPrice,
                    Profit = round((result[0]["BuyPriceUSD"]-row.AdjustPrice)/row.AdjustPrice*100, 2),
                    UpdateDatetime = datetime.now().strftime('%Y-%m-%d %H:%M:%S')
                )
            else:
                query = update(Stocks).where(Stocks.ListingID == row.ListingID).values(
                        OrderNo = row.OrderNo,
                        AdjustPrice = row.AdjustPrice,
                        UpdateDatetime = datetime.now().strftime('%Y-%m-%d %H:%M:%S')
                )
            db.execute(query)
        db.commit()

    return JSONResponse(
        status_code=status.HTTP_200_OK,
        content={
            "message": "ok"
        }
    )

@router.delete("/{ListingID}")
async def deletestock(ListingID: str):
    with get_db() as db:
        query = update(Stocks).filter(Stocks.ListingID == ListingID).values(
            IsDelete = "T"
        )
        db.execute(query)
        db.commit()

    return JSONResponse(
        status_code=status.HTTP_200_OK,
        content={
            "message": "ok"
        }
    )

@router.get("/listing/{ListingID}")
async def getstock(ListingID: str):
    with get_db() as db:
        results = db.query(Stocks).filter(and_(Stocks.ListingID == ListingID, Stocks.IsDelete == "F")).all()

    return results

@router.patch("/listing")
async def patchstock(request: stocks_schema.RequestPatchListing):
    BuyPriceUSD = 0
    if request.BuyPrice > 0:
        BuyPriceUSD = request.BuyPrice/1300
    with get_db() as db:
        query = update(Stocks).filter(Stocks.ListingID == request.ListingID).values(
            BuyPrice = request.BuyPrice,
            BuyPriceUSD = BuyPriceUSD,
            Price = request.Price,
            Limit = request.Limit,
            UpdateDatetime = datetime.now().strftime('%Y-%m-%d %H:%M:%S')
        )
        db.execute(query)
        db.commit()

    return JSONResponse(
        status_code=status.HTTP_202_ACCEPTED,
        content={
            "message": "accepted"
        }
    )

@router.patch("/listing/price")
async def patchstock_price(request: stocks_schema.RequestPatchListingPrice):
    with get_db() as db:
        query = update(Stocks).filter(Stocks.ListingID == request.ListingID).values(
            Price = int(request.Price)
        )
        db.execute(query)
        db.commit()

    return JSONResponse(
        status_code = status.HTTP_200_OK,
        content = {
            "message": "ok"
        }
    )