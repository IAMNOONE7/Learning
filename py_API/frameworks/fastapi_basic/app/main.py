"""
This module creates the FastAPI application.

WHY this file exists:
- It is the entry point for the application.
- Keeps the server setup, metadata, and router registration in one place.
"""

"""
 ====================
 Application assembly
 ====================
    - “Create the FastAPI app”
    - “Attach routers”
    - “Expose app to uvicorn”

 - Does not touch:
    - SQL
    - models
    - business logic
"""

from fastapi import FastAPI
from .routes import books, auth
from .cache import close_redis
import logging


def create_app() -> FastAPI:
    """
    Factory function that creates and configures the FastAPI app.

    WHY use a factory:
    - Cleaner structure
    - Easier testing (we can create multiple app instances)
    - Common pattern in production apps
    """
    app = FastAPI(
        title="FastAPI Basic – Books API",
        version="0.1.0",
        description="Learning project: basic REST API with FastAPI (Books & Reviews domain).",
    )

    app.include_router(auth.router)
    # Register routers – this keeps the code modular.
    app.include_router(books.router, prefix="/books", tags=["books"])

    logging.basicConfig(
        level=logging.INFO,
        format="%(asctime)s %(levelname)s %(name)s: %(message)s",
    )

    # Simple endpoint used for uptime checks / monitoring.
    @app.get("/health", tags=["system"])
    async def health_check() -> dict:
        return {"status": "ok"}

    @app.on_event("shutdown")
    async def _shutdown():
        await close_redis()

    return app


# The ASGI application used by Uvicorn.
app = create_app()
