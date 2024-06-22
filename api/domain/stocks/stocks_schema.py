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
    Status: str
    # CreateDatetime: AwareDatetime
    # UpdateDatetime: AwareDatetime | None = None

# class StocksExcelExport(BaseModel):
#     StyleID: str
#     Title: str
#     VariantValue: str
#     ListingID: str

# class RequestAddStocksBulk(BaseModel):
#     Data: list[RequestAddStocks]

# class ResponseStocks(BaseModel):
#     Data: list[Stocks] | None = None

class RequestPatchOrder(BaseModel):
    OrderNo: str
    ListingID: str
    AdjustPrice: float

class RequestPatchListing(BaseModel):
    ListingID: str
    BuyPrice: int
    Price: int
    Limit: int
    Status: str
    
class RequestPatchBuyPrice(BaseModel):
    ListingID: str
    BuyPrice: str