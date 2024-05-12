from fastapi import FastAPI
from starlette.middleware.cors import CORSMiddleware

from domain.user import user_router
from domain.stocks import stocks_router
from domain.callback import callback_router

app = FastAPI()
# stockx_callback_router = APIRouter()

# origins=[
#     "http://127.0.0.1:8000"
# ]
# app.add_middleware(
#     CORSMiddleware,
#     allow_origins=origins,
#     allow_credentials=True,
#     allow_methods=["*"],
#     allow_headers=["*"],
# )

# @app.get("/")
# def hello():
#     return "hello fastapi"

app.include_router(user_router.router)
app.include_router(stocks_router.router)
app.include_router(callback_router.router)

# @stockx_callback_router.post(
#     "{$callback_url}/api/callback/{$request.body.id}"
# )
# def test():
#     pass

