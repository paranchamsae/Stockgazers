# 모델 클래스 정의

from sqlalchemy import Boolean, Column, Integer, String, DateTime, ForeignKey, Float
from sqlalchemy.orm import relationship

from database import Base

class User(Base):
    __tablename__ = "SGUser"

    ID = Column(Integer, primary_key=True)
    IsDelete = Column(String, nullable=False, default=False)
    UserID = Column(String, nullable=False)
    UserPassword = Column(String, nullable=False)
    Email = Column(String, nullable=True)
    DiscountType = Column(String, nullable=False)
    CreateDatetime = Column(DateTime, nullable=False)
    UpdateDatetime = Column(DateTime, nullable=True)
    Tier = Column(Integer, nullable=False)
    IsAutoDiscount = Column(String, nullable=False)
    ExchangeRate = Column(String, nullable=False)
    DiscountPrice = Column(String, nullable=False)

class Stocks(Base):
    __tablename__ = "SGStocks"

    ID = Column(Integer, primary_key=True)
    UserID = Column(Integer, ForeignKey("SGUser.ID"), nullable=False)
    IsDelete = Column(String, nullable=False)
    ListingID = Column(String, nullable=False)
    StyleId = Column(String, nullable=False)
    ProductID = Column(String, nullable=False)
    Title = Column(String, nullable=False)
    VariantID = Column(String, nullable=False)
    VariantValue = Column(String, nullable=False)
    BuyPrice = Column(Integer, nullable=False)
    BuyPriceUSD = Column(Float, nullable=False, default=0)
    Price = Column(Integer, nullable=False, default=0)
    Limit = Column(Integer, nullable=False, default=0)
    Status = Column(String, nullable=True)
    OrderNo = Column(String, nullable=True)
    SellDatetime = Column(DateTime, nullable=True)
    SendDatetime = Column(DateTime, nullable=True)
    AdjustPrice = Column(Float, nullable=True)
    Profit = Column(Float, nullable=True)
    CreateDatetime = Column(DateTime, nullable=False)
    UpdateDatetime = Column(DateTime, nullable=True)



# class UserSettings(Base):
#     __tablename__ = "SGUserSettings"

#     UserID = Column(Integer, ForeignKey("sguser.id"))
#     IsAutoDiscount = Column(String, nullable=False, default="F")
#     ExchangeRate = Column(Integer, nullable=False, default=1300)
#     DiscountPrice = Column(Integer, nullable=False, default=0)

