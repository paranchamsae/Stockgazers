from fastapi import APIRouter, HTTPException, status
from fastapi.responses import JSONResponse
from database import get_db
from datetime import datetime

from sqlalchemy import select
from models import Stocks

router = APIRouter(
    prefix = "/api/stocks",
    tags=["Stocks"]
)

@router.get("/{UserID}", summary="내 재고 현황 불러오기")
async def get_stocks(UserID: str):
    with get_db() as db:
        result = db.query(Stocks).filter(Stocks.UserID == int(UserID)).all()

    return result