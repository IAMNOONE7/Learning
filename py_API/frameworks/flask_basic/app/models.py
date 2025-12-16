"""
Database models (Flask-SQLAlchemy).

In Flask:
- SQLAlchemy models live here
- They describe table structure only (no HTTP, no request logic)
"""

from .db import db  # Flask-SQLAlchemy instance


class Book(db.Model):
    """ORM model for the 'books' table."""
    __tablename__ = "books"

    id = db.Column(db.Integer, primary_key=True, index=True)         # PK column
    title = db.Column(db.String(200), nullable=False)                # required
    author = db.Column(db.String(100), nullable=False)               # required
    year = db.Column(db.Integer, nullable=True)                      # optional
    description = db.Column(db.Text, nullable=True)                  # optional

    def to_dict(self) -> dict:
        """Serialize ORM -> JSON-safe dict (Flask doesn't do this automatically)."""
        return {
            "id": self.id,
            "title": self.title,
            "author": self.author,
            "year": self.year,
            "description": self.description,
        }
