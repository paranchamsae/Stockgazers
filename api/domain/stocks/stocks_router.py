#-*- coding:utf-8 -*-
from fastapi import APIRouter, HTTPException, status, File, UploadFile
from fastapi.responses import JSONResponse, FileResponse
from fastapi.encoders import jsonable_encoder

from database import get_db
from datetime import datetime

import requests

from io import BytesIO, StringIO

from sqlalchemy import select, text, update, and_
from models import Stocks, Variants

from domain.stocks import stocks_schema

router = APIRouter(
    prefix = "/api/stocks",
    tags=["Stocks"]
)

def GetRatio():
    headers = {'User-Agent': 'Mozilla/5.0 (Macintosh; Intel Mac OS X 10_11_5) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/50.0.2661.102 Safari/537.36'}
    url = 'https://quotation-api-cdn.dunamu.com/v1/forex/recent?codes=FRX.KRWUSD'
    exchange =requests.get(url, headers=headers).json()
    return exchange[0]['basePrice']

@router.post("", summary="", status_code=status.HTTP_201_CREATED)
async def addStocks(request: list[stocks_schema.RequestAddStocks]):
    with get_db() as db:
        for row in request:
            tempBuyPriceRatio = 0
            tempBuyPriceUSD = 0
            if row.BuyPrice > 0:        # 구매원가 KRW가 입력되었다면 현재 환율 정보를 기반으로 구매원가 USD를 계산
                tempBuyPriceRatio = GetRatio()
                tempBuyPriceUSD = row.BuyPrice / tempBuyPriceRatio

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
                BuyPriceRatio = tempBuyPriceRatio,      # 구매입찰 등록 당시의 환율 정보
                BuyPriceUSD = round(tempBuyPriceUSD, 2),    # 구매원가 USD 계산 결과, 소숫점 둘째자리까지 저장
                Price = row.Price,      # 입찰 등록가
                Limit = row.Limit,      # 가격 하한선
                OrderNo = row.OrderNo,
                SellDatetime = row.SellDatetime,
                SendDatetime = row.SendDatetime,
                AdjustPrice = row.AdjustPrice,
                AdjustRatio = 0,        # 새로운 재고가 추가되는 경우에는 정산 당시 환율 정보가 없다.
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
        query = select(Stocks, Variants).join(Variants, Stocks.VariantValue==Variants.VariantValue).filter(and_(Stocks.UserID == int(UserID), Stocks.IsDelete == "F"))
        result = db.execute(query).mappings().all()

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
            tempAdjustRatio = GetRatio() if result[0].AdjustRatio == 0 else result[0].AdjustRatio       # 현재 실시간 환율
            if result[0].BuyPrice > 0 and result[0].BuyPriceUSD > 0:        # 구매원가 데이터가 입력된 경우 정산 데이터를 업데이트
                
                tempAdjustPrice = row.AdjustPrice/tempAdjustRatio if row.AdjustPrice > 10000 else row.AdjustPrice   # 정산금액 USD
                tempProfit = (tempAdjustPrice-result[0].BuyPriceUSD)/result[0].BuyPriceUSD*100      # 이익률
                if result[0].Status == "MATCHED" or result[0].Status == "READY_TO_SHIP":
                    tempProfit = 0
                    
                query = update(Stocks).where(Stocks.ListingID == row.ListingID).values(
                    OrderNo = row.OrderNo,
                    AdjustPrice = tempAdjustPrice,
                    AdjustRatio = tempAdjustRatio,
                    # 이익률 = 차익/구매원가*100 = (정산금액-구매원가)/구매원가*100
                    Profit = round( tempProfit, 2 ),
                    UpdateDatetime = datetime.now().strftime('%Y-%m-%d %H:%M:%S')
                )
            else:       # 구매원가 데이터가 입력되지 않은 경우
                if result[0].AdjustRatio == 0:   # 체결 당시의 환율 정보가 없는 경우에만 PATCH 동장
                    query = update(Stocks).where(Stocks.ListingID == row.ListingID).values(
                        OrderNo = row.OrderNo,
                        AdjustPrice = tempAdjustPrice,
                        AdjustRatio = tempAdjustRatio,
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

