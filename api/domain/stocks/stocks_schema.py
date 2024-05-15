from pydantic import BaseModel, AwareDatetime
from models import Stocks

class RequestAddStocks(BaseModel):
    UserID: int
    IsDelete: str
    ListingID: str
    StyleID: str
    ProductID: str
    Title: str
    VariantID: str
    VariantValue: str
    BuyPrice: int
    BuyPriceUSD: float
    Price: int
    Limit: int
    OrderNo: str
    SellDatetime: AwareDatetime | None = None
    SendDatetime: AwareDatetime | None = None
    AdjustPrice: float
    Profit: float
    # CreateDatetime: AwareDatetime
    # UpdateDatetime: AwareDatetime | None = None

# class RequestAddStocksBulk(BaseModel):
#     Data: list[RequestAddStocks]

# class ResponseStocks(BaseModel):
#     Data: list[Stocks] | None = None