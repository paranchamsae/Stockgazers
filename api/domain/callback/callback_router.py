from fastapi import Request, APIRouter, status
from fastapi.responses import JSONResponse
from fastapi.encoders import jsonable_encoder

router = APIRouter(
    prefix="/api/callback"
)

@router.post("")
async def callback(request: Request):
    # if request == None:
    #     return status.HTTP_200_OK
    
    return JSONResponse(
        status_code=status.HTTP_200_OK,
        content={
            "message": "ok"
        }
    )