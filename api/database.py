# 데이터베이스와 관련된 설정을 하는 파일
# 데이터베이스를 사용하기 위한 변수, 함수 등을 정의하고 접속할 데이터베이스의 주소, 사용자, 패스워드 등을 관리
import contextlib

from sqlalchemy import create_engine
from sqlalchemy.ext.declarative import declarative_base
from sqlalchemy.orm import sessionmaker

# import pymysql

SQLALCHEMY_DATABASE_URL = "mysql+pymysql://daniel:1q2w3e4r!@125.133.232.114:4144/stockgazers"

engine = create_engine(
    SQLALCHEMY_DATABASE_URL
)

# PYMYSQL_CONNECTION_STRING = "host='125.133.232.114', port='4144', user='daniel', passwd='1q2w3e4r!', db='stockgazers', charset='utf8'"
# engine = pymysql.connect(PYMYSQL_CONNECTION_STRING)
# cursor = engine.cursor()

SessionLocal = sessionmaker(autocommit=False, autoflush=False, bind=engine)

Base = declarative_base()

@contextlib.contextmanager
def get_db():
    db = SessionLocal()

    try:
        yield db
    finally:
        db.close()