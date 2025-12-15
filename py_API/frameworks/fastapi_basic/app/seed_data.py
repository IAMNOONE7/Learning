"""
Seeds the database with initial data.

Run with:
    python -m app.seed_data
"""

import asyncio

from sqlalchemy.ext.asyncio import AsyncSession

from .db import AsyncSessionLocal, engine
from .models import BookORM


BOOKS = [
    {
        "title": "Clean Code",
        "author": "Robert C. Martin",
        "year": 2008,
        "description": "A handbook of agile software craftsmanship.",
    },
    {
        "title": "Design Patterns",
        "author": "Erich Gamma et al.",
        "year": 1994,
        "description": "Elements of reusable object-oriented software.",
    },
    {
        "title": "The Pragmatic Programmer",
        "author": "Andrew Hunt, David Thomas",
        "year": 1999,
        "description": "From journeyman to master.",
    },
    {
        "title": "Introduction to Algorithms",
        "author": "Thomas H. Cormen",
        "year": 2009,
        "description": "The famous CLRS algorithms book.",
    },
    {
        "title": "Effective Python",
        "author": "Brett Slatkin",
        "year": 2015,
        "description": "59 specific ways to write better Python.",
    },
    {
        "title": "Clean Architecture",
        "author": "Robert C. Martin",
        "year": 2017,
        "description": "A guide to software architecture and design principles.",
    },
    {
        "title": "Refactoring",
        "author": "Martin Fowler",
        "year": 1999,
        "description": "Improving the design of existing code.",
    },
    {
        "title": "Domain-Driven Design",
        "author": "Eric Evans",
        "year": 2003,
        "description": "Tackling complexity in the heart of software.",
    },
    {
        "title": "Patterns of Enterprise Application Architecture",
        "author": "Martin Fowler",
        "year": 2002,
        "description": "Enterprise application design patterns.",
    },
    {
        "title": "Working Effectively with Legacy Code",
        "author": "Michael Feathers",
        "year": 2004,
        "description": "Techniques for safely changing legacy systems.",
    },
    {
        "title": "You Don’t Know JS Yet",
        "author": "Kyle Simpson",
        "year": 2020,
        "description": "Deep dive into JavaScript core mechanisms.",
    },
    {
        "title": "Python Cookbook",
        "author": "David Beazley, Brian K. Jones",
        "year": 2013,
        "description": "Recipes for mastering Python 3.",
    },
    {
        "title": "Fluent Python",
        "author": "Luciano Ramalho",
        "year": 2015,
        "description": "Clear, concise, and effective Python programming.",
    },
    {
        "title": "Head First Design Patterns",
        "author": "Eric Freeman, Elisabeth Robson",
        "year": 2004,
        "description": "A brain-friendly guide to design patterns.",
    },
    {
        "title": "The Mythical Man-Month",
        "author": "Frederick P. Brooks Jr.",
        "year": 1975,
        "description": "Essays on software engineering and project management.",
    },
    {
        "title": "Structure and Interpretation of Computer Programs",
        "author": "Harold Abelson, Gerald Jay Sussman",
        "year": 1996,
        "description": "Foundational concepts of computer science.",
    },
    {
        "title": "Code Complete",
        "author": "Steve McConnell",
        "year": 2004,
        "description": "A practical handbook of software construction.",
    },
    {
        "title": "The Clean Coder",
        "author": "Robert C. Martin",
        "year": 2011,
        "description": "A code of conduct for professional programmers.",
    },
    {
        "title": "Test-Driven Development: By Example",
        "author": "Kent Beck",
        "year": 2002,
        "description": "How to write tests first and design better code.",
    },
    {
        "title": "Programming Pearls",
        "author": "Jon Bentley",
        "year": 1986,
        "description": "Problem-solving techniques for programmers.",
    },
]



async def seed_books(session: AsyncSession) -> None:
    """
    Insert seed data into the books table.
    """
    books = [BookORM(**data) for data in BOOKS]
    session.add_all(books)
    await session.commit()


async def main() -> None:
    async with AsyncSessionLocal() as session:
        await seed_books(session)

    # Clean shutdown
    await engine.dispose()


if __name__ == "__main__":
    asyncio.run(main())
