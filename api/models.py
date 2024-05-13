# 모델 클래스 정의

from sqlalchemy import Boolean, Column, Integer, String, DateTime, ForeignKey
from sqlalchemy.orm import relationship

from database import Base

class User(Base):
    __tablename__ = "SGUser"

    ID = Column(Integer, primary_key=True)
    IsDelete = Column(String, nullable=False, default=False)
    UserID = Column(String, nullable=False)
    UserPassword = Column(String, nullable=False)
    CreateDatetime = Column(DateTime, nullable=False)
    UpdateDatetime = Column(DateTime, nullable=True)
    IsAutoDiscount = Column(String, nullable=False)
    ExchangeRate = Column(String, nullable=False)
    DiscountPrice = Column(String, nullable=False)

# class Stocks(Base):
#     __tablename__ = "SGStocks"

# class UserSettings(Base):
#     __tablename__ = "SGUserSettings"

#     UserID = Column(Integer, ForeignKey("sguser.id"))
#     IsAutoDiscount = Column(String, nullable=False, default="F")
#     ExchangeRate = Column(Integer, nullable=False, default=1300)
#     DiscountPrice = Column(Integer, nullable=False, default=0)

