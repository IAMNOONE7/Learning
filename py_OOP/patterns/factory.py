"""
factory.py
----------
Factory Pattern in Python.

We will implement:
1. Simple Factory
2. Factory Method Pattern
3. Abstract Factory Pattern
4. Real-world examples
"""

from abc import ABC, abstractmethod


# ----------------------------------------------------------
# EXAMPLE 1: Simple Factory
# ----------------------------------------------------------

class Dog:
    def speak(self):
        return "Woof!"

class Cat:
    def speak(self):
        return "Meow!"

class AnimalFactory:
    """
    Simple Factory: create object based on input.
    """
    @staticmethod
    def create_animal(animal_type: str):
        animal_type = animal_type.lower()
        if animal_type == "dog":
            return Dog()
        elif animal_type == "cat":
            return Cat()
        else:
            raise ValueError(f"Unknown animal type: {animal_type}")


# ----------------------------------------------------------
# EXAMPLE 2: Factory Method Pattern
# ----------------------------------------------------------

class Transport(ABC):
    @abstractmethod
    def deliver(self):
        pass


class Truck(Transport):
    def deliver(self):
        return "Delivering by land using a truck."


class Ship(Transport):
    def deliver(self):
        return "Delivering by sea using a ship."


class Logistics(ABC):
    """
    The base class expects subclasses to define the factory method.
    """

    @abstractmethod
    def create_transport(self) -> Transport:
        pass

    def plan_delivery(self):
        """
        High-level code that works with any created transport.
        """
        transport = self.create_transport()
        return f"Planning: {transport.deliver()}"


class RoadLogistics(Logistics):
    def create_transport(self) -> Transport:
        return Truck()


class SeaLogistics(Logistics):
    def create_transport(self) -> Transport:
        return Ship()


# ----------------------------------------------------------
# EXAMPLE 3: Abstract Factory Pattern
# ----------------------------------------------------------

class Button(ABC):
    @abstractmethod
    def render(self):
        pass


class WinButton(Button):
    def render(self):
        return "Windows Button"

class MacButton(Button):
    def render(self):
        return "MacOS Button"


class Checkbox(ABC):
    @abstractmethod
    def render(self):
        pass


class WinCheckbox(Checkbox):
    def render(self):
        return "Windows Checkbox"

class MacCheckbox(Checkbox):
    def render(self):
        return "MacOS Checkbox"


class GUIFactory(ABC):
    @abstractmethod
    def create_button(self) -> Button:
        pass

    @abstractmethod
    def create_checkbox(self) -> Checkbox:
        pass


class WinFactory(GUIFactory):
    def create_button(self) -> Button:
        return WinButton()

    def create_checkbox(self) -> Checkbox:
        return WinCheckbox()


class MacFactory(GUIFactory):
    def create_button(self) -> Button:
        return MacButton()

    def create_checkbox(self) -> Checkbox:
        return MacCheckbox()


# ----------------------------------------------------------
# REAL-WORLD EXAMPLE: Database Factory
# ----------------------------------------------------------

class Database(ABC):
    @abstractmethod
    def connect(self):
        pass

class MySQL(Database):
    def connect(self):
        return "Connecting to MySQL"

class PostgreSQL(Database):
    def connect(self):
        return "Connecting to PostgreSQL"

class SQLite(Database):
    def connect(self):
        return "Connecting to SQLite"


class DatabaseFactory:
    @staticmethod
    def get_database(db_type: str) -> Database:
        db_type = db_type.lower()
        match db_type:
            case "mysql":
                return MySQL()
            case "postgresql":
                return PostgreSQL()
            case "sqlite":
                return SQLite()
            case _:
                raise ValueError(f"Unknown DB type: {db_type}")


# ----------------------------------------------------------
# Example usage
# ----------------------------------------------------------
if __name__ == "__main__":
    print("=== Simple Factory ===")
    a1 = AnimalFactory.create_animal("dog")
    a2 = AnimalFactory.create_animal("cat")
    print(a1.speak())
    print(a2.speak())

    print("\n=== Factory Method Pattern ===")
    road = RoadLogistics()
    sea = SeaLogistics()
    print(road.plan_delivery())
    print(sea.plan_delivery())

    print("\n=== Abstract Factory Pattern ===")
    win_gui = WinFactory()
    mac_gui = MacFactory()

    print(win_gui.create_button().render())
    print(win_gui.create_checkbox().render())

    print(mac_gui.create_button().render())
    print(mac_gui.create_checkbox().render())

    print("\n=== Database Factory ===")
    db = DatabaseFactory.get_database("postgresql")
    print(db.connect())
