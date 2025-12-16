"""
Pydantic schemas (validation).

Flask does NOT auto-validate request bodies like FastAPI.
We manually parse request JSON -> Pydantic -> use validated data.
"""

from typing import Optional
from pydantic import BaseModel, Field


class BookBase(BaseModel):
    title: str = Field(..., min_length=1, max_length=200)
    author: str = Field(..., min_length=1, max_length=100)
    year: Optional[int] = Field(None, ge=0, le=2100)
    description: Optional[str] = Field(None, max_length=1000)


class BookCreate(BookBase):
    pass


class BookUpdate(BaseModel):
    title: Optional[str] = Field(None, min_length=1, max_length=200)
    author: Optional[str] = Field(None, min_length=1, max_length=100)
    year: Optional[int] = Field(None, ge=0, le=2100)
    description: Optional[str] = Field(None, max_length=1000)
