from fastapi import APIRouter, HTTPException, status
from fastapi.responses import JSONResponse
# from database import SessionLocal
from database import get_db
from datetime import datetime

from domain.user import user_schema

from sqlalchemy import select, insert, update, delete
from sqlalchemy.sql import text

from models import User

router = APIRouter(
    prefix = "/api/user",
    tags=["User"]
)

### TODO: sqlalchemy 스타일로 쿼리 변경!
@router.post("", summary="Stockgazers 클라이언트 계정 생성", status_code=status.HTTP_201_CREATED)
async def createUser(request: user_schema.RequestCreateUser):
    with get_db() as db:
        result = db.query(User).filter(User.UserID == request.LoginID).all()
        if len(result) > 0:     # 이미 존재하는 계정 정보라면 http 409 conflict를 리턴
            raise HTTPException(status_code=status.HTTP_409_CONFLICT, detail="이미 사용 중인 아이디입니다.")
        
        result = db.execute(text("select sha2('"+request.Password+"', 256)"))
        shapw = result.first()
        
        # record creation on SGUser 
        new_user = User(
            IsDelete = "F",
            UserID = request.LoginID,
            UserPassword = shapw[0],
            CreateDatetime = datetime.now().strftime('%Y-%m-%d %H:%M:%S'),
            UpdateDatetime = None,
            IsAutoDiscount = "F",
            ExchangeRate = 1300,
            DiscountPrice = 1,
            Tier = int(request.Tier),
            Email = request.Email,
            DiscountType = request.DiscountType
        )
        
        db.add(new_user)
        db.commit()

    return JSONResponse(
        status_code=status.HTTP_201_CREATED,
        content={
            "message": "created"
        }
    )

@router.get("/{userid}", summary="Stockgazers 클라이언트 접속자 정보", response_model=user_schema.ResponseUserInfo)
async def getUser(userid: int):
    with get_db() as db:
        result = db.query(User.ID.label("UserID"),
                          User.UserID.label("LoginID"),
                          User.ExchangeRate,
                          User.IsAutoDiscount,
                          User.DiscountPrice,
                          User.Tier,
                          User.DiscountType).filter(User.ID==userid).all()
        
        if len(result) < 1:
            raise HTTPException(status.HTTP_404_NOT_FOUND)
        elif len(result) == 1:  
            for row in result:      # 이게 최선일까
                element = row._asdict()
                response = user_schema.ResponseUserInfo(
                    UserID = element["UserID"],
                    LoginID=element["LoginID"],
                    ExchangeRate=element["ExchangeRate"],
                    IsAutoDiscount=element["IsAutoDiscount"],
                    DiscountPrice=element["DiscountPrice"],
                    Tier = element["Tier"],
                    DiscountType = element["DiscountType"]
                )
    return response

@router.post("/login", summary="Stockgazers 클라이언트 로그인")
async def login(request: user_schema.RequestLogin):
    statusCode = 0
    message = ""
    data = None
    core = None

    # db = SessionLocal()

    with get_db() as db:
        statement = text("select * from SGUser where UserID='"+request.ID+"'")      # 아이디로 디비에서 레코드 조회
        result = db.execute(statement)

        row = result.mappings().all()

        if len(row) < 1:        # 레코드 카운트가 1보다 작으면 없는 계정
            raise HTTPException(status.HTTP_404_NOT_FOUND, detail="noaccount")
        elif len(row) == 1:     # 레코드가 존재함
            if row[0]["IsDelete"] == "T":       # 그런데 삭제된 계정임
                raise HTTPException(status.HTTP_404_NOT_FOUND, detail="deleted account")
            else:
                statement = text("select sha2('"+request.PW+"', 256) as v")      # 해당 계정의 패스워드 일치 여부 확인
                pw_result = db.execute(statement)
                pwrow = pw_result.mappings().all()

                if pwrow[0]["v"] != row[0]["UserPassword"]:  # 패스워드 불일치
                    raise HTTPException(status.HTTP_404_NOT_FOUND, detail="invalid data")
                else:   # 패스워드도 일치함!
                    statusCode = 200
                    data = row[0]
                    statement = text("select * from SGKey order by id desc limit 1")
                    keyresult = db.execute(statement)
                    core = keyresult.mappings().all()[0]
                    # core = [tuple(keyrow) for keyrow in keyresult]
                    # print(core)

    ### TODO: JSONResponse type return implements
    # return JSONResponse(
    #     status_code=status.HTTP_200_OK,
    #     content={
    #         "statusCode": 200,
    #         "message": str(message),
    #         "data": data,
    #         "core": core
    #     }   
    # )
    return { "statusCode": statusCode, "message": str(message), "data": data, "core": core }

@router.patch("/preference", summary="개인 환경설정값 업데이트")
async def preference(request: user_schema.Preference):
    discountEnum = "T" if request.isAutoDiscount == True else "F"       # enum 필드 업데이트용 계산
    
    # 개인 설정값 변경 시작
    with get_db() as db:
        query = update(User).where(User.ID==request.userid).values(
            ExchangeRate=request.exchangeRate,
            IsAutoDiscount=discountEnum,
            DiscountPrice = request.autoDiscountPrice,
            UpdateDatetime = datetime.now().strftime('%Y-%m-%d %H:%M:%S'),
        )
        try:
            db.execute(query)
        except Exception as e:
            db.rollback()
        finally:
            db.commit()

    return JSONResponse(
        status_code=status.HTTP_200_OK,
        content={
            "message": "OK"
        }
    )
