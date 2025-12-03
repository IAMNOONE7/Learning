"""
abstract_classes.py
-------------------
Abstraction and "interfaces" in Python using:

- Abstract Base Classes (ABC)
- Abstract methods
- Interface-like design
- Polymorphism via abstract types
"""

from abc import ABC, abstractmethod
from typing import Protocol


# ----------------------------------------------------------
# EXAMPLE 1: Abstract Base Class as an interface
# ----------------------------------------------------------

class PaymentProcessor(ABC):
    """
    Abstract base class representing a generic payment processor.
    Acts like an "interface" in other languages.

    Any subclass MUST implement:
    - authorize()
    - capture()
    """

    @abstractmethod
    def authorize(self, amount: float) -> bool:
        """Check if the payment can be authorized."""
        pass

    @abstractmethod
    def capture(self, amount: float) -> None:
        """Actually charge the money."""
        pass


class CreditCardProcessor(PaymentProcessor):
    def __init__(self, card_number: str):
        self.card_number = card_number
        self.authorized_amount = 0.0

    def authorize(self, amount: float) -> bool:
        # In a real system, there would be API calls here.
        print(f"[CreditCard] Authorizing {amount} for card {self.card_number}")
        self.authorized_amount = amount
        return True

    def capture(self, amount: float) -> None:
        if amount > self.authorized_amount:
            raise ValueError("Cannot capture more than authorized amount.")
        print(f"[CreditCard] Capturing {amount} from card {self.card_number}")
        self.authorized_amount -= amount


class PaypalProcessor(PaymentProcessor):
    def __init__(self, account_email: str):
        self.account_email = account_email
        self.balance = 0.0

    def authorize(self, amount: float) -> bool:
        print(f"[PayPal] Authorizing {amount} for account {self.account_email}")
        # Simulate success
        self.balance += amount
        return True

    def capture(self, amount: float) -> None:
        if amount > self.balance:
            raise ValueError("Not enough authorized balance.")
        print(f"[PayPal] Capturing {amount} from {self.account_email}")
        self.balance -= amount


def process_order(processor: PaymentProcessor, amount: float):
    """
    Function using abstraction:
    - It doesn't care *which* payment method is used.
    - It only relies on the interface: authorize() + capture().
    """
    if processor.authorize(amount):
        processor.capture(amount)
        print("Payment completed.\n")
    else:
        print("Payment failed.\n")


# ----------------------------------------------------------
# EXAMPLE 2: Abstraction for report generation
# ----------------------------------------------------------

class ReportExporter(ABC):
    """
    Abstract class for exporting reports in different formats.
    """

    @abstractmethod
    def export(self, data: dict, filename: str) -> None:
        """Export the given data to a file."""
        pass


class CSVExporter(ReportExporter):
    def export(self, data: dict, filename: str) -> None:
        print(f"[CSV] Exporting {data} to {filename}.csv")
        # Here you would actually open a file and write CSV.


class JSONExporter(ReportExporter):
    def export(self, data: dict, filename: str) -> None:
        print(f"[JSON] Exporting {data} to {filename}.json")
        # Here you would actually open a file and write JSON.


def save_report(exporter: ReportExporter):
    """
    Another example of polymorphism via abstraction.
    """
    sample_data = {"total": 100, "items": 5}
    exporter.export(sample_data, "report")


# ----------------------------------------------------------
# EXAMPLE 3: Protocols (typing-based interfaces, Pythonic way)
# ----------------------------------------------------------

class SupportsSpeak(Protocol):
    """
    A Protocol is a "structural" interface:
    Any object with a .speak() -> str method is considered SupportsSpeak.
    No inheritance required.
    """

    def speak(self) -> str:
        ...


class Dog:
    def speak(self) -> str:
        return "Woof!"


class Robot:
    def speak(self) -> str:
        return "Beep boop."


def announce(entity: SupportsSpeak):
    """
    This works for ANY object that has a .speak() method,
    regardless of inheritance.
    """
    print("Announcement:", entity.speak())


# ----------------------------------------------------------
# Example usage
# ----------------------------------------------------------
if __name__ == "__main__":
    print("=== Payment Processors (ABC) ===")
    cc = CreditCardProcessor("4111 1111 1111 1111")
    pp = PaypalProcessor("user@example.com")

    process_order(cc, 50.0)
    process_order(pp, 75.0)

    print("=== Report Exporters (ABC) ===")
    csv_exporter = CSVExporter()
    json_exporter = JSONExporter()

    save_report(csv_exporter)
    save_report(json_exporter)

    print("=== Protocols (Structural Typing) ===")
    dog = Dog()
    robot = Robot()

    announce(dog)
    announce(robot)
