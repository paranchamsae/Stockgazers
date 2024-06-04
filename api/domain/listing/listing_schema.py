from typing import Optional
from pydantic import BaseModel

class RequestPatchListingPrice(BaseModel):
    ListingID: str
    Price: str