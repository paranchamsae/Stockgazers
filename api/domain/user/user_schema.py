from typing import Optional
from pydantic import BaseModel

#request
class RequestCreateUser(BaseModel):
    userid: str
    pw: str

class RequestLogin(BaseModel):
    ID: str
    PW: str

class Preference(BaseModel):
    userid: int
    pw: str
    exchangeRate: int
    isAutoDiscount: bool
    autoDiscountPrice: int

#response
