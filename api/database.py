# 데이터베이스와 관련된 설정을 하는 파일
# 데이터베이스를 사용하기 위한 변수, 함수 등을 정의하고 접속할 데이터베이스의 주소, 사용자, 패스워드 등을 관리

from sqlalchemy import create_engine
from sqlalchemy.ext.declarative import declarative_base
from sqlalchemy.orm import sessionmaker

# SQLALCHEMY_DATABASE_URL = "mysql+pymysql://daniel:4144@125.133.232.114:4144/stockgazers"

engine = create_engine(
    # SQLALCHEMY_DATABASE_URL
)

SessionLocal = sessionmaker(autocommit=False, autoflush=False, bind=engine)

Base = declarative_base()