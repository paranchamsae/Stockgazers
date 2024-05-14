from pydantic import BaseModel
from models import Stocks

class ResponseStocks(BaseModel):
    Data: list[Stocks] | None = None