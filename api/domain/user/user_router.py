from fastapi import APIRouter
# from database import SessionLocal
#from models import User

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