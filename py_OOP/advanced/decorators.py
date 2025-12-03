"""
decorators.py
--------------
Introduction to Python decorators.

Topics covered:
- Functions as first-class citizens
- Simple decorator
- Decorator with arguments
- Multiple decorators
- Using functools.wraps (very important!)
- Practical decorators (logging, timing)
- Method decorators inside classes
"""

import time
from functools import wraps


# ----------------------------------------------------------
# BASIC CONCEPT: Functions are first-class citizens
# ----------------------------------------------------------

def greet(name):
    return f"Hello, {name}!"

say_hello = greet  # Assign function to variable


# ----------------------------------------------------------
# EXAMPLE 1: Simple decorator
# ----------------------------------------------------------

def simple_decorator(func):
    """
    A decorator is a function that takes another function
    and returns a new function with added behavior.
    """

    @wraps(func)  # preserves original function name/docs
    def wrapper(*args, **kwargs):
        print("[DEBUG] Before function call")
        result = func(*args, **kwargs)
        print("[DEBUG] After function call")
        return result

    return wrapper


@simple_decorator
def say_message(msg):
    print("Message:", msg)


# ----------------------------------------------------------
# EXAMPLE 2: Decorator with arguments
# ----------------------------------------------------------

def repeat(n):
    """Run the decorated function n times."""

    def decorator(func):
        @wraps(func)
        def wrapper(*args, **kwargs):
            for _ in range(n):
                func(*args, **kwargs)
        return wrapper

    return decorator


@repeat(3)
def beep():
    print("Beep!")


# ----------------------------------------------------------
# EXAMPLE 3: Practical decorator: timing
# ----------------------------------------------------------

def measure_time(func):
    @wraps(func)
    def wrapper(*args, **kwargs):
        start = time.perf_counter()
        result = func(*args, **kwargs)
        end = time.perf_counter()
        print(f"[TIME] {func.__name__} took {(end - start):.6f} seconds")
        return result
    return wrapper


@measure_time
def slow_function():
    time.sleep(0.5)
    return "Done!"


# ----------------------------------------------------------
# EXAMPLE 4: Decorators for access control
# ----------------------------------------------------------

current_user_role = "admin"

def require_role(role):
    """Check if the user has permission."""

    def decorator(func):
        @wraps(func)
        def wrapper(*args, **kwargs):
            if current_user_role != role:
                print(f"[ACCESS DENIED] Required role: {role}")
                return None
            return func(*args, **kwargs)
        return wrapper

    return decorator


@require_role("admin")
def delete_database():
    print("Database deleted!")


# ----------------------------------------------------------
# EXAMPLE 5: Method decorator in a class
# ----------------------------------------------------------

def log_method(func):
    @wraps(func)
    def wrapper(self, *args, **kwargs):
        print(f"[LOG] Calling method: {func.__name__} of {self.__class__.__name__}")
        return func(self, *args, **kwargs)
    return wrapper


class Calculator:
    @log_method
    def add(self, a, b):
        return a + b


# ----------------------------------------------------------
# Example usage
# ----------------------------------------------------------

if __name__ == "__main__":
    print("=== Simple Decorator ===")
    say_message("Hello Decorator")

    print("\n=== Decorator with Arguments ===")
    beep()

    print("\n=== Timing Decorator ===")
    print(slow_function())

    print("\n=== Access Control Decorator ===")
    delete_database()

    print("\n=== Method Decorator ===")
    calc = Calculator()
    print(calc.add(3, 5))
