from fastapi import Request, APIRouter, status
from fastapi.responses import JSONResponse
from fastapi.encoders import jsonable_encoder

import requests

router = APIRouter(
    prefix="/api/callback"
)

@router.post("")
async def callback(request: Request):
    # if request == None:
    #     return status.HTTP_200_OK
    
    return JSONResponse(
        status_code=status.HTTP_200_OK,
        content={
            "message": "ok"
        }
    )

@router.get("/test")
async def TestCurrencyRate():
    headers = {'User-Agent': 'Mozilla/5.0 (Macintosh; Intel Mac OS X 10_11_5) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/50.0.2661.102 Safari/537.36'}
    url = 'https://quotation-api-cdn.dunamu.com/v1/forex/recent?codes=FRX.KRWUSD'
    exchange =requests.get(url, headers=headers).json()
    return exchange[0]['basePrice']