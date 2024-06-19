from typing import Optional
from pydantic import BaseModel

class RequestPatchListingPrice(BaseModel):
    ListingID: str
    Price: str

class RequestPatchListingStatus(BaseModel):
    ListingID: str
    Status: str