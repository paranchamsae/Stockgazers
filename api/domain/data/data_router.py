import math
from fastapi import APIRouter, HTTPException, status, File, UploadFile
from fastapi.responses import JSONResponse, FileResponse
# from database import SessionLocal
from database import get_db
from datetime import datetime

from domain.user import user_schema
from domain.stocks import stocks_schema
from pandas import pandas       # excel export library

from sqlalchemy import select, insert, update, delete
from sqlalchemy.sql import text

from models import Stocks

router = APIRouter(
    prefix = "/api/data",
    tags=["Data"]
)

@router.get("/export/{UserID}", summary="내 재고 현황 엑셀로 내려받기(구매원가 업로드용)")
async def get_stocks_excel(UserID: int):
    elements = []
    with get_db() as db:
        result = db.query(Stocks).filter(Stocks.UserID == UserID).all()
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

    filename = "data-"+str(UserID)+"-"+datetime.now().strftime('%Y%m%d')+".csv"
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
