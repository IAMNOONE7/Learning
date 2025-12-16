"""
Books endpoints (Flask Blueprint).

This file defines ALL HTTP behavior related to books:
- routing (URLs + HTTP methods)
- parsing query parameters
- calling the database
- returning JSON responses
"""

from __future__ import annotations
from typing import Any
from flask import Blueprint, jsonify, request
# Blueprint = Flask's equivalent of FastAPI's APIRouter
# jsonify  = creates proper JSON responses with headers
# request  = global request object (query params, body, headers)

from pydantic import ValidationError
# Flask does NOT validate request bodies automatically
# We must call Pydantic manually and handle errors ourselves

from sqlalchemy import asc, desc
from ..db import db
from ..models import Book
from ..schemas import BookCreate, BookUpdate


# ================================================================
# Blueprint definition
# ================================================================

books_bp = Blueprint(
    "books",          # internal name of blueprint
    __name__,         # Python module name
    url_prefix="/books"  # all routes start with /books
)

# Equivalent to:
# router = APIRouter(prefix="/books") in FastAPI


# ================================================================
# Helper functions
# ================================================================

def _int_arg(name: str, default: int | None = None) -> int | None:
    """
    Safely read an integer query parameter.

    Why?
    - request.args returns strings
    - Users can send invalid values (?page=abc)
    - We don't want the whole endpoint to crash

    Example:
        page = _int_arg("page", 1)
    """
    raw = request.args.get(name)
    if raw is None or raw == "":
        return default
    try:
        return int(raw)
    except ValueError:
        return default


def _parse_sort(sort_by: str, sort_dir: str, allowed: dict[str, Any]):
    """
    Parse and validate sorting parameters.

    Supported formats:
        sort_by=author
        sort_by=author,title
        sort_by=author:desc,title:asc

    Why allowlist?
    - Prevent SQL injection via ORDER BY
    - Only known columns can be sorted
    """
    default_desc = (sort_dir or "asc").lower() == "desc"
    order_exprs = []

    # Split comma-separated fields
    parts = [p.strip() for p in (sort_by or "id").split(",") if p.strip()]

    for part in parts:
        if ":" in part:
            field, direction = part.split(":", 1)
            field = field.strip()
            direction = direction.strip().lower()
            use_desc = direction == "desc"
        else:
            field = part
            use_desc = default_desc

        # Map user input -> actual ORM column
        col = allowed.get(field)
        if not col:
            continue  # silently ignore unknown fields

        order_exprs.append(desc(col) if use_desc else asc(col))

    # Fallback sorting (stable pagination)
    if not order_exprs:
        order_exprs = [asc(allowed["id"])]

    return order_exprs


def _error(status_code: int, message: str, details: Any | None = None):
    """
    Standard JSON error response.

    Flask does not have HTTPException like FastAPI,
    so we return (json, status_code) manually.
    """
    payload = {"error": message}
    if details is not None:
        payload["details"] = details
    return jsonify(payload), status_code


# ================================================================
# Routes
# ================================================================

@books_bp.get("")
def list_books():
    """
    GET /books

    Responsibilities:
    - parse query parameters
    - apply filtering
    - apply sorting
    - apply pagination
    - return consistent JSON shape

    Flask differences vs FastAPI:
    - No Depends()
    - No Query()
    - Everything comes from request.args
    """

    # Pagination
    page = _int_arg("page", 1) or 1
    page_size = _int_arg("page_size", 10) or 10

    # Enforce sane limits
    page = max(page, 1)
    page_size = min(max(page_size, 1), 100)

    offset = (page - 1) * page_size

    # Filtering params
    title_contains = request.args.get("title_contains")
    author = request.args.get("author")
    year_from = _int_arg("year_from")
    year_to = _int_arg("year_to")

    # Sorting params
    sort_by = request.args.get("sort_by", "id")
    sort_dir = request.args.get("sort_dir", "asc")

    # Base query
    q = Book.query
    # Flask-SQLAlchemy gives us this shortcut
    # Equivalent to session.query(Book)


    # Apply filters
    if title_contains:
        q = q.filter(Book.title.ilike(f"%{title_contains}%"))

    if author:
        q = q.filter(Book.author.ilike(author))

    if year_from is not None:
        q = q.filter(Book.year >= year_from)

    if year_to is not None:
        q = q.filter(Book.year <= year_to)

    # Total count BEFORE pagination
    total = q.count()
    # Sorting
    allowed_sorts = {
        "id": Book.id,
        "title": Book.title,
        "author": Book.author,
        "year": Book.year,
    }
    order_exprs = _parse_sort(sort_by, sort_dir, allowed_sorts)
    q = q.order_by(*order_exprs)

    # Pagination
    q = q.offset(offset).limit(page_size)

    # Execute query
    items = [book.to_dict() for book in q.all()]

    # Response
    return jsonify({
        "items": items,
        "page": page,
        "page_size": page_size,
        "total": total,
    })


@books_bp.get("/<int:book_id>")
def get_book(book_id: int):
    """
    GET /books/<id>

    Flask does path conversion via <int:book_id>
    """
    book = Book.query.get(book_id)
    if not book:
        return _error(404, f"Book with id={book_id} not found")

    return jsonify(book.to_dict())


@books_bp.post("")
def create_book():
    """
    POST /books

    Flask differences:
    - request.get_json() instead of automatic body parsing
    - Pydantic validation is manual
    """
    data = request.get_json(silent=True) or {}

    try:
        payload = BookCreate(**data)
    except ValidationError as e:
        return _error(422, "Validation error", e.errors())

    book = Book(
        title=payload.title,
        author=payload.author,
        year=payload.year,
        description=payload.description,
    )

    db.session.add(book)
    db.session.commit()

    return jsonify(book.to_dict()), 201


@books_bp.put("/<int:book_id>")
def update_book(book_id: int):
    """
    PUT /books/<id>

    Partial update:
    - Only fields sent by the client are updated
    """
    book = Book.query.get(book_id)
    if not book:
        return _error(404, f"Book with id={book_id} not found")

    data = request.get_json(silent=True) or {}

    try:
        payload = BookUpdate(**data)
    except ValidationError as e:
        return _error(422, "Validation error", e.errors())

    update_data = payload.model_dump(exclude_unset=True)
    for field, value in update_data.items():
        setattr(book, field, value)

    db.session.commit()

    return jsonify(book.to_dict())


@books_bp.delete("/<int:book_id>")
def delete_book(book_id: int):
    """
    DELETE /books/<id>
    """
    book = Book.query.get(book_id)
    if not book:
        return _error(404, f"Book with id={book_id} not found")

    db.session.delete(book)
    db.session.commit()

    return "", 204
