from typing import Optional
from pydantic import BaseModel

#request
class RequestCreateUser(BaseModel):
    LoginID: str
    Password: str
    Email: str
    DiscountType: str
    Tier: str

class RequestLogin(BaseModel):
    ID: str
    PW: str

class Preference(BaseModel):
    userid: int
    # pw: str
    exchangeRate: int
    isAutoDiscount: bool
    autoDiscountPrice: int

#response
class ResponseUserInfo(BaseModel):
    UserID: int
    LoginID: str
    ExchangeRate: int | None = None
    IsAutoDiscount: bool | None = None
    DiscountPrice: int | None = None
    Tier: int
    DiscountType: str