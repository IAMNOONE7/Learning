"""
magic_methods.py
----------------
Demonstration of Python's magic (dunder) methods.

Topics covered:
- __str__ and __repr__
- Arithmetic operators (__add__, __sub__)
- Comparison (__eq__, __lt__)
- Container behavior (__len__, __getitem__)
- Context manager (__enter__, __exit__)
- Practical examples
"""


# ----------------------------------------------------------
# EXAMPLE 1: __str__ and __repr__
# ----------------------------------------------------------

class Person:
    def __init__(self, name: str, age: int):
        self.name = name
        self.age = age

    def __str__(self):
        """User-friendly string (what print() shows)."""
        return f"{self.name}, {self.age} years old"

    def __repr__(self):
        """Developer-friendly string (debugging, logs)."""
        return f"Person(name={self.name!r}, age={self.age!r})"


# ----------------------------------------------------------
# EXAMPLE 2: Arithmetic magic methods
# ----------------------------------------------------------

class Vector:
    def __init__(self, x: float, y: float):
        self.x = x
        self.y = y

    def __add__(self, other):
        return Vector(self.x + other.x, self.y + other.y)

    def __sub__(self, other):
        return Vector(self.x - other.x, self.y - other.y)

    def __repr__(self):
        return f"Vector({self.x}, {self.y})"


# ----------------------------------------------------------
# EXAMPLE 3: Comparisons
# ----------------------------------------------------------

class Score:
    def __init__(self, value: int):
        self.value = value

    def __eq__(self, other):
        return self.value == other.value

    def __lt__(self, other):
        return self.value < other.value

    def __repr__(self):
        return f"Score({self.value})"


# ----------------------------------------------------------
# EXAMPLE 4: Make your object behave like a container
# ----------------------------------------------------------

class ShoppingCart:
    def __init__(self, items):
        self.items = items

    def __len__(self):
        """Allows using len(cart)."""
        return len(self.items)

    def __getitem__(self, index):
        """Allows indexing: cart[0]."""
        return self.items[index]

    def __repr__(self):
        return f"ShoppingCart({self.items})"


# ----------------------------------------------------------
# EXAMPLE 5: Context manager (__enter__, __exit__)
# ----------------------------------------------------------

class FileManager:
    """
    A custom context manager for opening/closing files.
    Allows usage: with FileManager(...) as f:
    """

    def __init__(self, filename, mode):
        self.filename = filename
        self.mode = mode
        self.file = None

    def __enter__(self):
        print("[ENTER] Opening file...")
        self.file = open(self.filename, self.mode)
        return self.file

    def __exit__(self, exc_type, exc_val, exc_tb):
        print("[EXIT] Closing file...")
        if self.file:
            self.file.close()
        return False  # Do not suppress exceptions


# ----------------------------------------------------------
# Example usage
# ----------------------------------------------------------
if __name__ == "__main__":
    print("=== __str__ and __repr__ ===")
    p = Person("Alice", 30)
    print(str(p))
    print(repr(p))

    print("\n=== Arithmetic magic methods ===")
    v1 = Vector(2, 3)
    v2 = Vector(5, -1)
    print("v1 + v2 =", v1 + v2)
    print("v1 - v2 =", v1 - v2)

    print("\n=== Comparison magic methods ===")
    s1 = Score(10)
    s2 = Score(20)
    print("s1 == s2:", s1 == s2)
    print("s1 < s2:", s1 < s2)

    print("\n=== Container behavior ===")
    cart = ShoppingCart(["Apples", "Bananas", "Milk"])
    print("Length:", len(cart))
    print("First item:", cart[0])

    print("\n=== Context manager ===")
    # Creates test_file.txt automatically
    with FileManager("test_file.txt", "w") as f:
        f.write("Hello world!")
