from fastapi import APIRouter, status
from database import SessionLocal

from domain.user import user_schema

from sqlalchemy import select, insert, update, delete
from sqlalchemy.sql import text

router = APIRouter(
    prefix = "/api/user",
    tags=["User"]
)

@router.post("", summary="Stockgazers 클라이언트 계정 생성", status_code=status.HTTP_201_CREATED)
async def createUser(request: user_schema.RequestCreateUser):
    return -1

@router.get("/{userid}", summary="Stockgazers 클라이언트 접속자 정보")
async def getUser(userid: int):    
    return userid

@router.post("/login", summary="Stockgazers 클라이언트 로그인")
async def login(request: user_schema.RequestLogin):
    statusCode = 0
    message = ""
    data = None
    core = None

    db = SessionLocal()

    statement = text("select * from SGUser where UserID='"+request.ID+"'")      # 아이디로 디비에서 레코드 조회
    result = db.execute(statement)

    row = result.mappings().all()

    if len(row) < 1:        # 레코드 카운트가 1보다 작으면 없는 계정
        statusCode = 404
        message = "noaccount"
    elif len(row) == 1:     # 레코드가 존재함
        if row[0]["IsDelete"] == "T":       # 그런데 삭제된 계정임
            statusCode = 404
            message = "deleted account"
        else:
            statement = text("select sha2('"+request.PW+"', 256) as v")      # 해당 계정의 패스워드 일치 여부 확인
            pw_result = db.execute(statement)
            pwrow = pw_result.mappings().all()

            if pwrow[0]["v"] != row[0]["UserPassword"]:  # 패스워드 불일치
                print(pwrow[0]["v"])
                print(row[0]["UserPassword"])
                statusCode = 404
                message = "invalid data"
            else:   # 패스워드도 일치함!
                statusCode = 200
                data = row[0]
                statement = text("select * from SGKey order by id desc limit 1")
                keyresult = db.execute(statement)
                core = keyresult.mappings().all()[0]

    return { "statusCode": statusCode, "message": str(message), "data": data, "core": core }

@router.put("/token", summary="Stockgazers 클라이언트 토큰 갱신")
async def updateToken():
    return -1

@router.patch("/preference", summary="개인 환경설정값 업데이트")
async def preference(request: user_schema.Preference):
    db = SessionLocal()

    # 패스워드 변경
    query = text("update SGUser set UserPassword=sha2('"+request.pw+"', 256) where ID='"+request.userid+"'")
    result = db.execute(query)

    # 개인 설정값 변경
    query = text("update SGUserSettings set ExchangeRate="+request.exchangeRate+", IsAutoDiscount='"+request.isAutoDiscount+"', DiscountPrice="+request.autoDiscountPrice+" where UserID="+request.userid+" ")
    db.execute(query)

    db.commit()

    return { "statusCode": 200, "message": "" }

# @router.get("/test")
# def test():
#     db = SessionLocal()
    
#     statement = text("select * from SGUser")
#     result = db.execute(statement)

#     for row in result.mappings():
#         print (row["UserID"])
#         ret = row["UserID"]

#     return ret