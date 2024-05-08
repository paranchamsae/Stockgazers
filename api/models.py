# 모델 클래스 정의

from sqlalchemy import Boolean, Column, Integer, String, DateTime
from sqlalchemy.orm import relationship

from .database import Base

class User(Base):
    __tablename__ = "SGUser"

    id = Column(Integer, primary_key=True)
    isDelete = Column(Boolean, nullable=False, default=False)
    userID = Column(String, nullable=False)
    password = Column(String, nullable=False)
    createDatetime = Column(DateTime, nullable=False)
    updateDatetime = Column(DateTime, nullable=True)

class Stocks(Base):
    __taablename__ = "SGStocks"

class Preference(Base):
    __tablename__ = "SGUserSettings"

