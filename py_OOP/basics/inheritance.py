"""
inheritance.py
---------------
Demonstrates object-oriented inheritance in Python.

Topics covered:
- Creating a base class and derived classes
- Using super() to call parent constructors
- Overriding methods
- Polymorphism
- Why inheritance is useful
"""

# ----------------------------------------------------------
# BASE CLASS: Animal
# ----------------------------------------------------------

class Animal:
    def __init__(self, name: str):
        self.name = name

    def speak(self) -> str:
        """Base behavior – will be overridden by subclasses."""
        return "Unknown sound"

    def move(self) -> str:
        """A generic method shared by all animals."""
        return f"{self.name} moves around."


# ----------------------------------------------------------
# DERIVED CLASS: Dog
# ----------------------------------------------------------

class Dog(Animal):
    def __init__(self, name: str, breed: str):
        # Call the parent constructor
        super().__init__(name)
        self.breed = breed

    # Override the speak() method
    def speak(self) -> str:
        return "Woof!"

    def fetch(self) -> str:
        """Child-specific behavior."""
        return f"{self.name} is fetching the ball."


# ----------------------------------------------------------
# DERIVED CLASS: Cat
# ----------------------------------------------------------

class Cat(Animal):
    def __init__(self, name: str, color: str):
        # Call the parent constructor
        super().__init__(name)
        self.color = color

    # Override the speak() method
    def speak(self) -> str:
        return "Meow!"

    def scratch(self) -> str:
        """Child-specific behavior."""
        return f"{self.name} is scratching the furniture."


# ----------------------------------------------------------
# POLYMORPHISM EXAMPLE
# ----------------------------------------------------------

def animal_speak(animal: Animal):
    """
    Polymorphism in action:
    - The same function works with ANY subclass of Animal.
    - The actual method called depends on the object's real type.
    """
    print(f"{animal.name} says: {animal.speak()}")


# ----------------------------------------------------------
# Example usage
# ----------------------------------------------------------
if __name__ == "__main__":
    dog = Dog("Buddy", "Golden Retriever")
    cat = Cat("Luna", "Gray")

    print("=== Single Object Behavior ===")
    print(dog.move())
    print(cat.move())

    print(dog.fetch())
    print(cat.scratch())

    print("\n=== Polymorphism ===")
    animal_speak(dog)  # Calls Dog.speak()
    animal_speak(cat)  # Calls Cat.speak()

    print("\n=== Overriding Demonstration ===")
    print(dog.speak())  # "Woof!"
    print(cat.speak())  # "Meow!"
