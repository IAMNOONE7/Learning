"""
encapsulation.py
----------------
Encapsulation in Python: protecting data and controlling access.

Topics covered:
- Public, protected (_var) and private (__var) attributes
- Name-mangling
- Getters and setters via @property
- Read-only properties
- Validation logic
"""

# ----------------------------------------------------------
# EXAMPLE 1: Basic encapsulation with protected and private attributes
# ----------------------------------------------------------

class BankAccount:
    def __init__(self, owner: str, balance: float):
        self.owner = owner                    # public attribute
        self._balance = balance               # protected attribute (by convention)
        self.__pin_code = "0000"              # private attribute (name-mangled)

    # Public method to access protected data safely
    def get_balance(self) -> float:
        return self._balance

    # Public method to modify protected data safely
    def deposit(self, amount: float):
        if amount <= 0:
            print("Deposit amount must be positive.")
            return
        self._balance += amount

    # Proper getter/setter for private attribute
    def set_pin(self, new_pin: str):
        if not new_pin.isdigit() or len(new_pin) != 4:
            print("PIN must be a 4-digit number.")
            return
        self.__pin_code = new_pin

    def get_pin(self):
        return self.__pin_code


# ----------------------------------------------------------
# EXAMPLE 2: Using @property (preferred in Python)
# ----------------------------------------------------------

class TemperatureSensor:
    """
    Demonstrates modern encapsulation using @property.
    This allows us to access attributes like normal variables,
    but with validation logic under the hood.
    """

    def __init__(self, celsius: float):
        self._celsius = celsius   # protected storage attribute

    # Getter
    @property
    def celsius(self) -> float:
        return self._celsius

    # Setter with validation
    @celsius.setter
    def celsius(self, value: float):
        if value < -273.15:
            raise ValueError("Temperature cannot go below absolute zero!")
        self._celsius = value

    # Read-only computed property
    @property
    def fahrenheit(self) -> float:
        return self._celsius * 9/5 + 32


# ----------------------------------------------------------
# Example usage
# ----------------------------------------------------------
if __name__ == "__main__":
    print("=== BankAccount Example ===")
    acc = BankAccount("Alice", 1000)
    acc.deposit(200)
    print("Balance:", acc.get_balance())

    acc.set_pin("1234")
    print("PIN:", acc.get_pin())

    # Access protected member (technically possible but discouraged)
    print("Accessing protected attribute:", acc._balance)

    # Private attributes are name-mangled:
    print("Private name-mangled:", acc._BankAccount__pin_code)

    print("\n=== TemperatureSensor Example ===")
    sensor = TemperatureSensor(25.0)
    print("Celsius:", sensor.celsius)
    print("Fahrenheit:", sensor.fahrenheit)

    # Setting a new value triggers the validation
    sensor.celsius = 100
    print("Updated Celsius:", sensor.celsius)
    print("Updated Fahrenheit:", sensor.fahrenheit)

    # Uncomment to see validation error:
    # sensor.celsius = -500  # Raises ValueError
