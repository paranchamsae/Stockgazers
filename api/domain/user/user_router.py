from fastapi import APIRouter
from database import SessionLocal
# from models import User

from sqlalchemy import select, insert, update, delete
from sqlalchemy.sql import text

router = APIRouter(
    prefix = "/api/user",
    tags=["User"]
)

@router.post("/login")
def login():
    #db = SessionLocal()
    #_test = db.query(User)
    #db.close()
    #return _test
    return {"message": "hello"}

@router.get("/test")
def test():
    db = SessionLocal()
    
    statement = text("select * from SGUser")
    result = db.execute(statement)

    for row in result.mappings():
        print (row["UserID"])
        ret = row["UserID"]

    return ret