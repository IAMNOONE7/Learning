"""
dataclasses_examples.py
------------------------
Practical examples of Python dataclasses.

Topics covered:
- Basic @dataclass usage
- Automatic __init__, __repr__, __eq__
- Default values and default_factory
- Frozen dataclasses (immutable)
- Post-init processing
- Nested dataclasses
- Field customizations
"""

from dataclasses import dataclass, field
from typing import List
import uuid


# ----------------------------------------------------------
# EXAMPLE 1: Basic dataclass
# ----------------------------------------------------------

@dataclass
class User:
    id: int
    name: str
    email: str


# ----------------------------------------------------------
# EXAMPLE 2: Dataclass with default values
# ----------------------------------------------------------

@dataclass
class Product:
    name: str
    price: float
    in_stock: bool = True  # default value


# ----------------------------------------------------------
# EXAMPLE 3: Default factory (must be a callable)
# ----------------------------------------------------------

@dataclass
class Order:
    order_id: str = field(default_factory=lambda: str(uuid.uuid4())) #unique identifier
    items: List[str] = field(default_factory=list)


# ----------------------------------------------------------
# EXAMPLE 4: Frozen dataclass (immutable object)
# ----------------------------------------------------------

@dataclass(frozen=True)
class Point:
    x: float
    y: float

    def distance_from_origin(self) -> float:
        return (self.x**2 + self.y**2) ** 0.5


# ----------------------------------------------------------
# EXAMPLE 5: post-init validation
# ----------------------------------------------------------

@dataclass
class Temperature:
    celsius: float

    def __post_init__(self):
        if self.celsius < -273.15:
            raise ValueError("Temperature below absolute zero!")


# ----------------------------------------------------------
# EXAMPLE 6: Nested dataclasses
# ----------------------------------------------------------

@dataclass
class Address:
    street: str
    city: str


@dataclass
class Customer:
    name: str
    address: Address


# ----------------------------------------------------------
# Example usage
# ----------------------------------------------------------
if __name__ == "__main__":
    print("=== Dataclass: Basic ===")
    user = User(1, "Alice", "alice@example.com")
    print(user)  # automatic __repr__

    print("\n=== Dataclass: Default values ===")
    product = Product("Laptop", 1500.0)
    print(product)

    print("\n=== Dataclass: Default factory ===")
    order = Order(items=["Keyboard", "Mouse"])
    print(order)

    print("\n=== Frozen Dataclass ===")
    point = Point(3, 4)
    print(point)
    print("Distance:", point.distance_from_origin())
    #point.x = 10  # ERROR: Cannot modify frozen dataclass

    print("\n=== Post-init validation ===")
    temp = Temperature(25)
    print(temp)
    #Temperature(-500)  # Raises ValueError

    print("\n=== Nested Dataclasses ===")
    addr = Address("Main St", "Prague")
    customer = Customer("Bob", addr)
    print(customer)
