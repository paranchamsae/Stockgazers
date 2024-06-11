from typing import Optional
from pydantic import BaseModel

class HighestSellData(BaseModel):
    StyleID: str
    Title: str
    Count: int
    
class ResponseStatistics(BaseModel):
    TotalRow: int
    LastMonthTotalRow: int
    ActiveRow: int
    LastMonthActiveRow: int
    MatchedRow: int
    AvgBuyPrice: float
    AvgAdjustPrice: float
    AvgProfit: float
    HighestSell: HighestSellData

