from __future__ import annotations

def add(a: float, b: float) -> float:
    """Return the sum of two numbers."""
    return a + b


def divide(a: float, b: float) -> float:
    """
    Divide a by b.

    Raises:
        ValueError: if b is zero.
    """
    if b == 0:
        raise ValueError("Division by zero is not allowed")
    return a / b


def is_prime(n: int) -> bool:
    """
    Return True if n is a prime number.

    Simple implementation; good enough for testing practice.
    """
    if n < 2:
        return False
    if n in (2, 3):
        return True
    if n % 2 == 0:
        return False

    i = 3
    while i * i <= n:
        if n % i == 0:
            return False
        i += 2
    return True
