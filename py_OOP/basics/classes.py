"""
classes.py
----------
Basic introduction to Python classes and OOP concepts.

Topics covered:
- Defining a class
- Constructor (__init__)
- Instance attributes vs class attributes
- Methods (behaviors)
- Encapsulation conventions (_protected, __private)
- __str__ for readable object printing
"""


# ----------------------------------------------------------
# CLASS: Car
# ----------------------------------------------------------
class Car:
    # Class attribute (shared by all Car objects)
    wheels = 4

    def __init__(self, brand: str, model: str, year: int):
        """
        Constructor (initializer)
        - Runs every time a new Car object is created.
        - Defines instance attributes (unique to each object).
        """
        self.brand = brand          # public attribute
        self.model = model          # public attribute
        self.year = year            # public attribute

        self._mileage = 0           # protected attribute (by convention)
        self.__engine_number = "UNKNOWN"   # private attribute (name-mangled)

    # ------------------------------------------------------
    # INSTANCE METHODS – define behavior
    # ------------------------------------------------------

    def drive(self, km: float):
        """Increase the mileage by a given distance."""
        if km <= 0:
            print("Distance must be positive.")
            return

        self._mileage += km
        print(f"{self.brand} {self.model} drove {km} km.")

    def get_mileage(self):
        """Return the protected mileage value."""
        return self._mileage

    def set_engine_number(self, number: str):
        """
        Setter for the "private" engine number.
        Private attributes use name-mangling: _Car__engine_number
        """
        self.__engine_number = number

    def get_engine_number(self):
        """Getter for the private engine number."""
        return self.__engine_number

    # ------------------------------------------------------
    # SPECIAL METHOD (dunder method)
    # ------------------------------------------------------

    def __str__(self):
        """
        __str__ gives a human-readable representation of the object.
        Called when using print(car).
        """
        return f"{self.brand} {self.model} ({self.year})"


# ----------------------------------------------------------
# Example usage
# ----------------------------------------------------------
if __name__ == "__main__":
    car1 = Car("Toyota", "Corolla", 2020)
    car2 = Car("BMW", "M3", 2018)

    print(car1)     # Calls __str__
    print(car2)

    # Drive the cars
    car1.drive(50)
    car1.drive(20)

    # Accessing protected attribute via method
    print("Mileage:", car1.get_mileage())

    # Working with private attribute via getters/setters
    car1.set_engine_number("ENG-2020-XYZ")
    print("Engine number:", car1.get_engine_number())

    # Class attribute shared by all objects
    print("All cars have wheels:", Car.wheels)

    # Drive the cars
    car1.drive(50)
    car1.drive(20)

    print("Mileage:", car1.get_mileage())
    print("Mileage:", car2.get_mileage())