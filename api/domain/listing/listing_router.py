from fastapi import APIRouter, HTTPException, status
from fastapi.responses import JSONResponse
# from database import SessionLocal
from database import get_db
from datetime import datetime

from domain.stocks import stocks_schema
from domain.listing import listing_schema

from sqlalchemy import select, insert, update, delete, and_
from sqlalchemy.sql import text

from models import User, Stocks

router = APIRouter(
    prefix = "/api/listing",
    tags=["Listing"]
)

@router.get("/{ListingID}")
async def getstock(ListingID: str):
    with get_db() as db:
        results = db.query(Stocks).filter(and_(Stocks.ListingID == ListingID, Stocks.IsDelete == "F")).all()

    return results

@router.patch("")
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
            Status = request.Status,
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

@router.patch("/price")
async def patchstock_price(request: listing_schema.RequestPatchListingPrice):
    
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