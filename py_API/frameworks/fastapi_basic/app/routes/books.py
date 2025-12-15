"""
API routes related to books, now backed by PostgreSQL via SQLAlchemy.
"""

"""
 ==================================
 Application logic at HTTP boundary
 ==================================
    - “When user calls GET /books → do this”
    - “When user calls POST /books → do that”

 - Uses:
    - DB session
    - ORM models
    - SQLAlchemy queries

 This is the first place where intent becomes action.
"""

from typing import List, Optional
from fastapi import APIRouter, Depends, HTTPException, status, Query
from sqlalchemy import select, func, asc, desc
from sqlalchemy.ext.asyncio import AsyncSession
import logging

from ..db import get_db # Dependency that provides a DB session per request
from ..models import Book, BookCreate, BookUpdate, BookORM, Page, UserORM # API schemas and ORM model
from ..deps import get_current_user, require_role # auth dependencies
from ..cache import get_cache_version, cache_get_json, cache_set_json, bump_cache_version


router = APIRouter() # Router instance to group book-related endpoints
log = logging.getLogger("cache")

def _parse_sort(sort_by: str, sort_dir: str, allowed_sorts: dict):
    default_desc = sort_dir.lower() == "desc"  # global direction fallback
    order_exprs = []  # order_by expressions to return

    parts = [p.strip() for p in sort_by.split(",") if p.strip()]  # split "a,b,c"

    for part in parts:
        if ":" in part:
            field, direction = part.split(":", 1)  # "author:desc" -> ("author", "desc")
            field = field.strip()
            direction = direction.strip().lower()
            use_desc = direction == "desc"
        else:
            field = part
            use_desc = default_desc

        col = allowed_sorts.get(field)  # map string -> ORM column
        if not col:
            continue  # ignore unknown fields (or raise 400 if you prefer)

        order_exprs.append(desc(col) if use_desc else asc(col))  # build safe ORDER BY expr

    if not order_exprs:
        order_exprs = [asc(allowed_sorts["id"])]  # stable fallback ordering

    return order_exprs

@router.get("/", response_model=Page[Book])  # Return a paginated response of Book items
async def list_books(
    db: AsyncSession = Depends(get_db),  # DB session injected by FastAPI
    page: int = Query(1, ge=1),  # Read ?page= from URL, default 1, must be >= 1
    page_size: int = Query(10, ge=1, le=100),  # Read ?page_size=, default 10, limit to 100
    title_contains: Optional[str] = Query(None, max_length=100),  # search substring in title
    author: Optional[str] = Query(None, max_length=100),  # filter by author
    year_from: Optional[int] = Query(None, ge=0, le=2100),  # filter year >=
    year_to: Optional[int] = Query(None, ge=0, le=2100),  # filter year <=
    sort_by: str = Query("id", max_length=200),  # which field to sort by (allowlisted below)
    sort_dir: str = Query("asc", pattern="^(asc|desc)$"),  # sort direction: asc/desc
) -> Page[Book]:  # The function returns a Page[Book]
    """
    Return all books from the database.
    """
    offset = (page - 1) * page_size  # Convert page/page_size to SQL OFFSET

    cache_version = await get_cache_version()  # cache invalidation version

    cache_key = (
        f"books:list:v{cache_version}:"
        f"page={page}:size={page_size}:"
        f"title={title_contains or ''}:author={author or ''}:"
        f"yf={year_from or ''}:yt={year_to or ''}:"
        f"sort={sort_by}:{sort_dir}"
    )

    cached = await cache_get_json(cache_key)  # check Redis first
    if cached is not None:
        log.info("REDIS HIT %s", cache_key)
        return Page[Book](**cached)  # return cached response without touching DB

    log.info("REDIS MISS %s", cache_key)
    # Build WHERE conditions based on query params (safe, parameterized)
    conditions = []  # collect filters here

    if title_contains:  # if user requested title search
        conditions.append(BookORM.title.ilike(f"%{title_contains}%"))  # ILIKE for case-insensitive contains

    if author:  # if user requested author filter
        conditions.append(BookORM.author.ilike(author))  # case-insensitive match

    if year_from is not None:  # lower bound filter
        conditions.append(BookORM.year >= year_from)

    if year_to is not None:  # upper bound filter
        conditions.append(BookORM.year <= year_to)

    # Safe sorting: map user input -> actual ORM column (prevents SQL injection via ORDER BY)
    allowed_sorts = {  # allowlist of sortable columns
        "id": BookORM.id,
        "title": BookORM.title,
        "author": BookORM.author,
        "year": BookORM.year,
    }
    order_by_exprs = _parse_sort(sort_by, sort_dir, allowed_sorts)   # build multi-field ORDER BY safely

    total_stmt = select(func.count()).select_from(BookORM)  # Build SELECT COUNT(*) FROM books
    if conditions:                                          # Check if any filters were provided
        total_stmt = total_stmt.where(*conditions)          # Apply the same WHERE filters to the count query
    total_result = await db.execute(total_stmt)             # Execute COUNT query against the database
    total = total_result.scalar_one()                       # Extract the single integer value (total matching rows)

    # item query (filters + multi-sort + paging)
    items_stmt = select(BookORM)
    if conditions:
        items_stmt = items_stmt.where(*conditions)          # Apply the same WHERE filters to the items query

    items_stmt = (
        items_stmt
        .order_by(*order_by_exprs)                          # Apply safe multi-field ORDER BY expressions
        .offset(offset)                                     # Skip rows from previous pages
        .limit(page_size)                                   # Limit number of rows returned for this page
    )

    result = await db.execute(items_stmt)  # execute items query
    books_orm = result.scalars().all()  # list of ORM objects

    items = [Book.model_validate(b) for b in books_orm]  # ORM -> Pydantic response schema

    page_obj = Page[Book](items=items, page=page, page_size=page_size, total=total) # stable page envelope

    await cache_set_json(cache_key, page_obj.model_dump(), ttl_seconds=30)  # short TTL for learning

    return page_obj


@router.get("/{book_id}", response_model=Book)
async def get_book(book_id: int, db: AsyncSession = Depends(get_db),) -> Book:
    """
    Return a single book by ID.
    """
    result = await db.execute( # Execute parameterized SELECT query
        select(BookORM).where(BookORM.id == book_id) # WHERE books.id == book_id
    )
    book_orm = result.scalar_one_or_none() # Get one row or None if not found

    if book_orm is None:
        raise HTTPException(
            status_code=status.HTTP_404_NOT_FOUND,
            detail=f"Book with id={book_id} not found",
        )

    return Book.model_validate(book_orm) # Convert ORM object to Pydantic model


@router.post("/", response_model=Book, status_code=status.HTTP_201_CREATED)
async def create_book(payload: BookCreate,
                      db: AsyncSession = Depends(get_db),
                      user: UserORM = Depends(get_current_user),
                      ) -> Book:
    """
    Create a new book in the database.
    """
    book_orm = BookORM( # Create ORM object from request payload
        title=payload.title,
        author=payload.author,
        year=payload.year,
        description=payload.description,
    )

    db.add(book_orm)  # Mark object for insertion
    await db.commit() # Commit transaction (INSERT happens here)
    await db.refresh(book_orm)  # Reload object to get generated ID from DB
    await bump_cache_version()
    return Book.model_validate(book_orm)


@router.put("/{book_id}", response_model=Book)
async def update_book(book_id: int, payload: BookUpdate,
                      db: AsyncSession = Depends(get_db),
                      user: UserORM = Depends(get_current_user),
                      ) -> Book:
    """
    Update an existing book.
    """
    result = await db.execute(
        select(BookORM).where(BookORM.id == book_id)
    )
    book_orm = result.scalar_one_or_none()

    if book_orm is None:
        raise HTTPException(
            status_code=status.HTTP_404_NOT_FOUND,
            detail=f"Book with id={book_id} not found",
        )

    # Apply changes only for provided fields
    data = payload.model_dump(exclude_unset=True) # Extract only fields provided by client
    for field, value in data.items():
        setattr(book_orm, field, value)

    await db.commit()  # Commit transaction (UPDATE happens here)
    await db.refresh(book_orm)
    await bump_cache_version()
    return Book.model_validate(book_orm)


@router.delete("/{book_id}", status_code=status.HTTP_204_NO_CONTENT)
async def delete_book(book_id: int,
                      db: AsyncSession = Depends(get_db),
                      user: UserORM = Depends(require_role("admin")),  # <-- admin only
                      ) -> None:
    """
    Delete a book.
    """
    result = await db.execute(
        select(BookORM).where(BookORM.id == book_id)
    )
    book_orm = result.scalar_one_or_none()

    if book_orm is None:
        raise HTTPException(
            status_code=status.HTTP_404_NOT_FOUND,
            detail=f"Book with id={book_id} not found",
        )

    await db.delete(book_orm)
    await db.commit()
    await bump_cache_version()
    return None
