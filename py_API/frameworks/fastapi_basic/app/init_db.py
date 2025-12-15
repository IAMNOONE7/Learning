"""
Helper script to create database tables.

Run once after setting DATABASE_URL and before using the API
"""

import asyncio

from .db import Base, engine
from .models import BookORM, UserORM, RefreshTokenORM  # import models so Base sees them

async def init_models() -> None:
    async with engine.begin() as conn:        
        # Drop all tables (careful!) and create them again
        # For a real project you’d use Alembic, but this is fine for learning.
        await conn.run_sync(Base.metadata.drop_all)
        await conn.run_sync(Base.metadata.create_all)


if __name__ == "__main__":
    asyncio.run(init_models())
