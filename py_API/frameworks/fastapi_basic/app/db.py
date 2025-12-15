"""
Database configuration using SQLAlchemy (async) + PostgreSQL.
Loads DSN from .env safely and builds an async SQLAlchemy URL.

We support DSN in this style (your example):
    PG_DSN=dbname=fastapi_books user=postgres password=xxx host=localhost port=5432
"""

"""
 =======================
 Database infrastructure
 =======================
    - “Here is how to connect”
    - “Here is how sessions are created”
    - “Here is how FastAPI gets a DB session”

- No idea about:
    - books
    - endpoints
    - business rules
"""

import os
from typing import AsyncGenerator, Dict
from dotenv import load_dotenv

from sqlalchemy.ext.asyncio import (
    async_sessionmaker,
    AsyncSession,
    create_async_engine,
)
from sqlalchemy.orm import DeclarativeBase


# ----------------------------------------------------------------------
# 1. Load .env only once
# ----------------------------------------------------------------------

_env_loaded = False

def _load_env_once() -> None:
    """
    Loads .env exactly once.
    Calling multiple times is safe.
    """
    global _env_loaded
    if not _env_loaded:
        load_dotenv(override=False)
        _env_loaded = True


# ----------------------------------------------------------------------
# 2. Parse PG_DSN from environment
# ----------------------------------------------------------------------

def _parse_pg_dsn(raw: str) -> Dict[str, str]:
    """
    Converts a DSN string like:
        "dbname=fastapi_books user=postgres password=123 host=localhost port=5432"
    into a dict:
        { "dbname": "fastapi_books", "user": "postgres", ... }

    WHY this function:
    - SQLAlchemy async engine requires a URL, not DSN.
    """
    parts = raw.strip().split()
    d = {}

    for p in parts:
        if "=" not in p:
            raise ValueError(f"Invalid DSN fragment: {p}")
        key, val = p.split("=", 1)
        d[key] = val

    required = ["dbname", "user", "password", "host", "port"]
    for r in required:
        if r not in d or not d[r]:
            raise ValueError(f"Missing '{r}' in PG_DSN")

    return d


def _build_asyncpg_url(dsn_dict: Dict[str, str]) -> str:
    """
    Builds a SQLAlchemy asyncpg URL:
        postgresql+asyncpg://user:pass@host:port/dbname
    """
    return (
        f"postgresql+asyncpg://"
        f"{dsn_dict['user']}:{dsn_dict['password']}"
        f"@{dsn_dict['host']}:{dsn_dict['port']}"
        f"/{dsn_dict['dbname']}"
    )


# ----------------------------------------------------------------------
# 3. Create engine + session factory
# ----------------------------------------------------------------------

def _get_database_url() -> str:
    """
    Reads PG_DSN from environment and returns a SQLAlchemy async URL.
    """
    _load_env_once()

    raw_dsn = os.getenv("PG_DSN")
    if not raw_dsn:
        raise RuntimeError(
            "Environment variable PG_DSN is not set. "
            "Add it to your .env file or system environment."
        )

    try:
        dsn_dict = _parse_pg_dsn(raw_dsn)
    except Exception as ex:
        raise RuntimeError(f"Failed to parse PG_DSN: {ex}") from ex

    return _build_asyncpg_url(dsn_dict)


DATABASE_URL = _get_database_url()


# ----------------------------------------------------------------------
# 4. SQLAlchemy Base, Engine, and Session
# ----------------------------------------------------------------------

class Base(DeclarativeBase):
    """Base for ORM models"""


engine = create_async_engine(
    DATABASE_URL,
    echo=True,  # log SQL queries for learning
)


AsyncSessionLocal = async_sessionmaker(
    engine,
    expire_on_commit=False,
)


async def get_db() -> AsyncGenerator[AsyncSession, None]:
    """
    FastAPI dependency that yields a database session per request.
    """
    async with AsyncSessionLocal() as session:
        yield session
