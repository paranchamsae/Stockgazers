from fastapi import APIRouter
# from database import SessionLocal
#from models import User

router = APIRouter(
    prefix = "/api/stocks",
    tags=["Stocks"]
)

@router.get("")
def login():
    #db = SessionLocal()
    #_test = db.query(User)
    #db.close()
    #return _test
    return {"message": "hello stocks router"}