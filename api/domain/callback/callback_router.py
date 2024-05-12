from fastapi import Request, APIRouter

router = APIRouter(
    prefix="/api/callback"
)

@router.post("")
async def callback(request: Request):
    if request == None:
        return None
    return request.json()