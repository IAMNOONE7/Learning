from __future__ import annotations
from typing import Dict


def is_positive(x: int) -> bool:
    """Return True if x is greater than zero."""
    return x > 0


def add(a: int, b: int) -> int:
    """Return the sum of two integers."""
    return a + b


def safe_divide(a: float, b: float) -> float:
    """
    Divide a by b.

    Raises:
        ValueError: if b is zero.
    """
    if b == 0:
        raise ValueError("division by zero")
    return a / b


def word_count(text: str) -> int:
    """Return the number of whitespace-separated words in the given text."""
    return len(text.split())


def make_user(name: str, active: bool = True) -> Dict[str, object]:
    """
    Build a simple user dictionary.

    Args:
        name: User's name.
        active: Whether the user is active.

    Returns:
        dict with keys 'name' and 'active'.
    """
    return {"name": name, "active": active}


def multiply(a: int, b: int) -> int:
    """Multiply two integers."""
    return a * b


def combine_strings(prefix: str, suffix: str) -> str:
    """Combine two strings with a dash in between."""
    return f"{prefix}-{suffix}"
