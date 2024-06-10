from fastapi import APIRouter, HTTPException, status
from fastapi.responses import JSONResponse
# from database import SessionLocal
from database import get_db
from datetime import datetime

from domain.user import user_schema

from sqlalchemy import select, insert, update, delete
from sqlalchemy.sql import text

from models import User

router = APIRouter(
    prefix = "/api/main",
    tags=["Main"]
)

@router.get("/statistics/{UserID}", summary="Stockgazers 접속자 판매통계")
async def get_statistics(UserID: int):
    query = "SELECT styleid, title, COUNT(*) AS cnt FROM SGStocks WHERE userid="+str(UserID)+" GROUP BY styleid, title ORDER BY COUNT(*) DESC LIMIT 1"
    query2 = """
        SELECT 
            COUNT(*) AS TotalRow,
            SUM(case when CreateDatetime<DATE_FORMAT(NOW(), '%Y-%m-01 00:00:00') then 1 ELSE 0 END) AS LastMonthTotalRow,
            sum(case when STATUS='ACTIVE' then 1 ELSE 0 END) AS ActiveRow,
            SUM(case when STATUS='ACTIVE' AND CreateDatetime<DATE_FORMAT(NOW(), '%Y-%m-01 00:00:00') then 1 ELSE 0 END) AS LastMonthActiveRow,
            sum(case when STATUS='MATCHED' then 1 ELSE 0 END) AS MatchedRow,
            AVG(BuyPrice) AS AvgBuyPrice,
            AVG(AdjustPrice) AS AvgAdjuctPrice,
            AVG(Profit) AS AvgProfit
        FROM SGStocks
        WHERE UserID="""+str(UserID)+""" AND IsDelete='F'
    """

    with get_db() as db:
        result = db.execute(text(query)).all()
        print(result)
        result = db.execute(text(query2)).all()
        print(result)

    return JSONResponse(
        status_code=status.HTTP_200_OK,
        content={
            "message": "ok",
            # "data": jsonable_encoder(result)
        }
    )