"""
Pydantic models (for API) and SQLAlchemy ORM models (for DB).
"""

"""
 ====================
 Data shape & schema
 ====================
    - “This is what a Book is”
    - “This is how it maps to a table”
 - No DB calls
 - No HTTP logic
 - Passive definitions only
"""


from typing import Optional, Generic, List, TypeVar

from pydantic import BaseModel, Field
from sqlalchemy import Integer, String, Text, DateTime, ForeignKey, Boolean
from sqlalchemy.orm import Mapped, mapped_column
from datetime import datetime

from .db import Base


# ---------- SQLAlchemy ORM model ----------

class BookORM(Base):
    """
    SQLAlchemy model for the 'books' table.

    This is what actually lives in PostgreSQL.
    """
    __tablename__ = "books"

    id: Mapped[int] = mapped_column(Integer, primary_key=True, index=True)
    title: Mapped[str] = mapped_column(String(200), nullable=False)
    author: Mapped[str] = mapped_column(String(100), nullable=False)
    year: Mapped[Optional[int]] = mapped_column(Integer, nullable=True)
    description: Mapped[Optional[str]] = mapped_column(Text, nullable=True)

class UserORM(Base):
    __tablename__ = "users"

    id: Mapped[int] = mapped_column(Integer, primary_key=True)
    username: Mapped[str] = mapped_column(String(50), unique=True, index=True, nullable=False)
    password_hash: Mapped[str] = mapped_column(String(255), nullable=False)
    role: Mapped[str] = mapped_column(String(20), nullable=False, default="user")  # "user" | "admin"


class RefreshTokenORM(Base):
    __tablename__ = "refresh_tokens"

    id: Mapped[int] = mapped_column(Integer, primary_key=True)
    jti: Mapped[str] = mapped_column(String(64), unique=True, index=True, nullable=False)
    user_id: Mapped[int] = mapped_column(ForeignKey("users.id"), nullable=False)
    expires_at: Mapped[datetime] = mapped_column(DateTime(timezone=True), nullable=False)
    revoked: Mapped[bool] = mapped_column(Boolean, nullable=False, default=False)

# ---------- Pydantic models (API schemas) ----------

class BookBase(BaseModel):
    title: str = Field(..., min_length=1, max_length=200)
    author: str = Field(..., min_length=1, max_length=100)
    year: Optional[int] = Field(None, ge=0, le=2100)
    description: Optional[str] = Field(None, max_length=1000)


class BookCreate(BookBase):
    """Data required to create a new book."""
    pass


class BookUpdate(BaseModel):
    """Data for partial update (all fields optional)."""
    title: Optional[str] = Field(None, min_length=1, max_length=200)
    author: Optional[str] = Field(None, min_length=1, max_length=100)
    year: Optional[int] = Field(None, ge=0, le=2100)
    description: Optional[str] = Field(None, max_length=1000)


class Book(BookBase):
    id: int

    class Config:
        # Allow Pydantic to work with ORM objects (BookORM)
        from_attributes = True

T = TypeVar("T")  # Generic type variable for items in a page

class Page(BaseModel, Generic[T]):  # Generic “page wrapper” schema
    items: List[T]  # The current page of items
    page: int  # Current page number (1-based)
    page_size: int  # Items per page
    total: int  # Total items in DB (for UI page count)


class UserCreate(BaseModel):
    username: str = Field(..., min_length=3, max_length=50)
    password: str = Field(..., min_length=6, max_length=100)

class UserPublic(BaseModel):
    id: int
    username: str
    role: str

    class Config:
        from_attributes = True

class TokenPair(BaseModel):
    access_token: str
    refresh_token: str
    token_type: str = "bearer"