from fastapi import APIRouter, HTTPException, status
from fastapi.responses import JSONResponse
from fastapi.encoders import jsonable_encoder
# from database import SessionLocal
from database import get_db
from datetime import datetime

from domain.user import user_schema
from domain.main import main_schema

from sqlalchemy import select, insert, update, delete
from sqlalchemy.sql import text

from models import User

router = APIRouter(
    prefix = "/api/main",
    tags=["Main"]
)

@router.get("/statistics/{UserID}", summary="Stockgazers 접속자 판매통계", response_model=main_schema.ResponseStatistics)
async def get_statistics(UserID: int):
    query = "SELECT StyleID, Title, COUNT(*) AS CNT FROM SGStocks WHERE userid="+str(UserID)+" GROUP BY styleid, title ORDER BY COUNT(*) DESC LIMIT 1"
    query2 = """
        SELECT 
            COUNT(*) AS TotalRow,
            SUM(case when CreateDatetime<DATE_FORMAT(NOW(), '%Y-%m-01 00:00:00') then 1 ELSE 0 END) AS LastMonthTotalRow,
            sum(case when STATUS='ACTIVE' then 1 ELSE 0 END) AS ActiveRow,
            SUM(case when STATUS='ACTIVE' AND CreateDatetime<DATE_FORMAT(NOW(), '%Y-%m-01 00:00:00') then 1 ELSE 0 END) AS LastMonthActiveRow,
            sum(case when STATUS='MATCHED' then 1 ELSE 0 END) AS MatchedRow,
            AVG(BuyPrice) AS AvgBuyPrice,
            AVG(AdjustPrice) AS AvgAdjustPrice,
            AVG(Profit) AS AvgProfit
        FROM SGStocks
        WHERE UserID="""+str(UserID)+""" AND IsDelete='F'
    """

    with get_db() as db:
        highest = db.execute(text(query)).all()
        # print(result)
        result = db.execute(text(query2)).all()
        # print(result[0].TotalRow)
        response = main_schema.ResponseStatistics(
            TotalRow=result[0].TotalRow,
            LastMonthTotalRow=result[0].LastMonthTotalRow,
            ActiveRow=result[0].ActiveRow,
            LastMonthActiveRow=result[0].LastMonthActiveRow,
            MatchedRow=result[0].MatchedRow,
            AvgBuyPrice=format(result[0].AvgBuyPrice, ".2f"),
            AvgAdjustPrice=format(result[0].AvgAdjustPrice, ".2f"),
            AvgProfit=format(result[0].AvgProfit, ".2f"),
            HighestSell=main_schema.HighestSellData(
                StyleID=highest[0].StyleID,
                Title=highest[0].Title,
                Count=highest[0].CNT
            )
        )
        

    return JSONResponse(
        status_code=status.HTTP_200_OK,
        content={
            "message": "ok",
            "data": jsonable_encoder(response)
        }
    )