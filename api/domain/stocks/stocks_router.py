from fastapi import APIRouter, HTTPException, status, File, UploadFile
from fastapi.responses import JSONResponse, StreamingResponse
import math
from database import get_db
from datetime import datetime

from pandas import pandas       # excel export library
from io import BytesIO

from sqlalchemy import select, update
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
    StyleIDs = []
    Titles = []
    VariantValues = []
    ListingIDs = []

    with get_db() as db:
        result = db.query(Stocks).filter(Stocks.UserID == int(UserID)).all()
        for row in result:
            StyleIDs.append(row.StyleId)
            Titles.append(row.Title)
            VariantValues.append(row.VariantValue)
            ListingIDs.append(row.ListingID)

    frame = pandas.DataFrame({
        '모델코드': StyleIDs, 
        '모델명': Titles,
        '사이즈': VariantValues,
        '고유아이디': ListingIDs,
        '구매원가' : ""
    })
   
    return StreamingResponse(
        iter([frame.to_csv(index=False)]),
        media_type="text/csv",
        headers={"Content-Disposition": "attachment;filename=data.csv"}
    )

@router.post("/import", summary="내 재고 현황 엑셀 업로드(구매원가 업로드용)")
async def set_stocks_excel(file: UploadFile = File(...)):
    print(file.filename)
    dataframe = pandas.read_csv(file.file)
    # print(dataframe)
    with get_db() as db:
        for row in dataframe.iterrows():
            # print(row.iloc[3])
            result = db.query(Stocks).filter(Stocks.ListingID==row[1][3]).all()
            if math.isnan(float(row[1][4])):
                continue

            query = update(Stocks).where(Stocks.ListingID == row[1][4]).values(
                BuyPrice = int(row[1][4]),
                BuyPriceUSD = int(row[1][4])*1300
            )
            
            try:
                db.execute(query)
            except Exception as e:
                db.rollback()
            finally:
                db.commit()

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