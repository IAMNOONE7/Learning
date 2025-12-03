"""
singleton.py
------------
Several ways to implement the Singleton design pattern in Python.

A Singleton ensures that:
- Only ONE instance of a class exists
- Every access returns the SAME instance

We will implement:
1. Basic Singleton using class variable
2. Decorator-based Singleton
3. Metaclass-based Singleton (cleanest, most Pythonic)
4. Thread-safe Singleton
"""

import threading


# ----------------------------------------------------------
# EXAMPLE 1: Classic Singleton with __new__
# ----------------------------------------------------------

class ClassicSingleton:
    _instance = None

    def __new__(cls, *args, **kwargs):
        """Control instance creation."""
        if cls._instance is None:
            cls._instance = super().__new__(cls)
        return cls._instance

    def __init__(self, value):
        self.value = value

    def __repr__(self):
        return f"ClassicSingleton(id={id(self)}, value={self.value})"


# ----------------------------------------------------------
# EXAMPLE 2: Singleton as a decorator
# ----------------------------------------------------------

def singleton(cls):
    instances = {}

    def get_instance(*args, **kwargs):
        if cls not in instances:
            instances[cls] = cls(*args, **kwargs)
        return instances[cls]

    return get_instance


@singleton
class DecoratedSingleton:
    def __init__(self, data):
        self.data = data

    def __repr__(self):
        return f"DecoratedSingleton(id={id(self)}, data={self.data})"


# ----------------------------------------------------------
# EXAMPLE 3: Metaclass Singleton (most Pythonic)
# ----------------------------------------------------------

class SingletonMeta(type):
    """
    A metaclass controls HOW a class is created.
    This metaclass stores instances and ensures only one exists.
    """

    _instances = {}

    def __call__(cls, *args, **kwargs):
        # Called when you do: MyClass()
        if cls not in cls._instances:
            cls._instances[cls] = super().__call__(*args, **kwargs)
        return cls._instances[cls]


class MetaSingleton(metaclass=SingletonMeta):
    def __init__(self, config):
        self.config = config

    def __repr__(self):
        return f"MetaSingleton(id={id(self)}, config={self.config})"


# ----------------------------------------------------------
# EXAMPLE 4: Thread-safe Singleton (for concurrency)
# ----------------------------------------------------------

class ThreadSafeSingleton:
    _instance = None
    _lock = threading.Lock()

    def __new__(cls, message):
        with cls._lock:
            if cls._instance is None:
                cls._instance = super().__new__(cls)
        return cls._instance

    def __init__(self, message):
        self.message = message

    def __repr__(self):
        return f"ThreadSafeSingleton(id={id(self)}, message={self.message})"


# ----------------------------------------------------------
# Example usage
# ----------------------------------------------------------
if __name__ == "__main__":
    print("=== Classic Singleton ===")
    a = ClassicSingleton(10)
    b = ClassicSingleton(99)
    print(a)
    print(b)  # same id → same instance

    print("\n=== Decorator Singleton ===")
    x = DecoratedSingleton("Hello")
    y = DecoratedSingleton("World")
    print(x)
    print(y)  # same instance

    print("\n=== Metaclass Singleton ===")
    m1 = MetaSingleton("configA")
    m2 = MetaSingleton("configB")
    print(m1)
    print(m2)

    print("\n=== Thread-safe Singleton ===")
    t1 = ThreadSafeSingleton("foo")
    t2 = ThreadSafeSingleton("bar")
    print(t1)
    print(t2)
