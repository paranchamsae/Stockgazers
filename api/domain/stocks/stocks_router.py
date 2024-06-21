#-*- coding:utf-8 -*-
from fastapi import APIRouter, HTTPException, status, File, UploadFile
from fastapi.responses import JSONResponse, FileResponse
from fastapi.encoders import jsonable_encoder
import math
from database import get_db
from datetime import datetime


from io import BytesIO, StringIO

from sqlalchemy import select, text, update, and_
from models import Stocks, Variants

from domain.stocks import stocks_schema

router = APIRouter(
    prefix = "/api/stocks",
    tags=["Stocks"]
)

@router.post("", summary="", status_code=status.HTTP_201_CREATED)
async def addStocks(request: list[stocks_schema.RequestAddStocks]):
    with get_db() as db:
        for row in request:
            new_stock = Stocks(
                UserID = row.UserID,
                IsDelete = "F",
                ListingID = row.ListingID,
                StyleID = row.StyleID,
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
                Status = "ACTIVE"
            )
            db.add(new_stock)
        db.commit()

    return JSONResponse(
        status_code=status.HTTP_201_CREATED,
        content={
            "message": "ok"
        }
    )

@router.get("/{UserID}", summary="내 입찰 현황 불러오기")
async def get_stocks(UserID: str):
    with get_db() as db:
        result = db.query(Stocks, Variants).join(Variants, Stocks.VariantValue == Variants.VariantValue).filter(and_(Stocks.UserID == int(UserID), Stocks.IsDelete == "F")).all()
        # print(select(Stocks, Variants.KRValue).join(Variants, Stocks.VariantValue == Variants.VariantValue).filter(and_(Stocks.UserID == int(UserID), Stocks.IsDelete == "F")))

    return result

@router.get("/{UserID}/{Status}", summary="입찰 상태값별 입찰 현황 불러오기")
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


@router.patch("/order", summary="판매 완료 데이터에 판매정보 업데이트")
async def patchorder(request: list[stocks_schema.RequestPatchOrder]):
    with get_db() as db:
        for row in request:
            # 해당 레코드에 구매원가 데이터가 있다면 profit 데이터도 계산하여 업데이트 같이 쳐줌
            result = db.query(Stocks).filter(Stocks.ListingID == row.ListingID).all()
            
            tempAdjustPrice = 0
            tempProfit = 0
            if result[0].BuyPrice > 0 and result[0].BuyPriceUSD > 0:
                tempAdjustPrice = row.AdjustPrice/1300 if row.AdjustPrice > 1000 else row.AdjustPrice
                tempProfit = (tempAdjustPrice-result[0].BuyPriceUSD)/result[0].BuyPriceUSD*100
                if result[0].Status == "MATCHED" or result[0].Status == "READY_TO_SHIP":
                    tempProfit = 0
                    
                query = update(Stocks).where(Stocks.ListingID == row.ListingID).values(
                    OrderNo = row.OrderNo,
                    AdjustPrice = tempAdjustPrice,
                    # 이익률 = 차익/구매원가*100 = (정산금액-구매원가)/구매원가*100
                    Profit = round( tempProfit, 2 ),
                    UpdateDatetime = datetime.now().strftime('%Y-%m-%d %H:%M:%S')
                )
            else:
                query = update(Stocks).where(Stocks.ListingID == row.ListingID).values(
                    OrderNo = row.OrderNo,
                    AdjustPrice = tempAdjustPrice,
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

