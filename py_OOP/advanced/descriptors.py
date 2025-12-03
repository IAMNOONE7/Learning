"""
descriptors.py
--------------
Descriptors give you control over what happens when you:

    obj.attr       # get attribute
    obj.attr = x   # set attribute
    del obj.attr   # delete attribute

A descriptor is any class that defines one or more of:
    - __get__(self, instance, owner)
    - __set__(self, instance, value)
    - __delete__(self, instance)

Important:
- Descriptors are typically defined as class attributes.
- One descriptor instance can manage the attribute for MANY objects.
"""


# ----------------------------------------------------------
# EXAMPLE 1: Simple logging descriptor
# ----------------------------------------------------------

class LoggedAttribute:
    """
    A descriptor that logs every get and set.
    """

    def __init__(self, name):
        # 'name' is the attribute name (for logging/human info)
        self.name = name
        # We need somewhere to store values per instance:
        # Use a dict mapping instance -> value.
        self._values = {}

    def __get__(self, instance, owner):
        if instance is None:
            # Accessed from the class, e.g. MyClass.x
            return self
        value = self._values.get(instance, None)
        print(f"[GET] {self.name} for {instance}: {value}")
        return value

    def __set__(self, instance, value):
        print(f"[SET] {self.name} for {instance} to {value}")
        self._values[instance] = value

    def __delete__(self, instance):
        print(f"[DEL] {self.name} for {instance}")
        if instance in self._values:
            del self._values[instance]


class DemoLogged:
    # One descriptor instance per attribute name
    x = LoggedAttribute("x")
    y = LoggedAttribute("y")

    def __repr__(self):
        return f"DemoLogged(id={id(self)})"


# ----------------------------------------------------------
# EXAMPLE 2: Validation descriptor (e.g. non-negative, typed)
# ----------------------------------------------------------

class PositiveNumber:
    """
    Descriptor enforcing that the value is a non-negative number.
    """

    def __init__(self, name):
        self.name = name
        self._values = {}

    def __get__(self, instance, owner):
        if instance is None:
            return self
        return self._values.get(instance, 0)

    def __set__(self, instance, value):
        if not isinstance(value, (int, float)):
            raise TypeError(f"{self.name} must be a number.")
        if value < 0:
            raise ValueError(f"{self.name} must be non-negative.")
        self._values[instance] = value

    def __delete__(self, instance):
        raise AttributeError(f"Cannot delete attribute {self.name}.")


class Account:
    """
    Account uses descriptors to validate 'balance' and 'credit_limit'.
    """

    balance = PositiveNumber("balance")
    credit_limit = PositiveNumber("credit_limit")

    def __init__(self, balance: float, credit_limit: float):
        # These assignments go through the descriptor __set__
        self.balance = balance
        self.credit_limit = credit_limit

    def __repr__(self):
        return (
            f"Account(balance={self.balance}, "
            f"credit_limit={self.credit_limit})"
        )


# ----------------------------------------------------------
# EXAMPLE 3: Type-checked descriptor using a single class
# ----------------------------------------------------------

class Typed:
    """
    Descriptor that enforces a specific type.
    """

    def __init__(self, name, expected_type):
        self.name = name
        self.expected_type = expected_type
        self._values = {}

    def __get__(self, instance, owner):
        if instance is None:
            return self
        return self._values.get(instance, None)

    def __set__(self, instance, value):
        if not isinstance(value, self.expected_type):
            raise TypeError(
                f"{self.name} must be of type {self.expected_type.__name__}, "
                f"got {type(value).__name__}"
            )
        self._values[instance] = value


class Person:
    # Same Typed descriptor class reused for multiple attributes
    name = Typed("name", str)
    age = Typed("age", int)

    def __init__(self, name: str, age: int):
        self.name = name   # goes through Typed.__set__
        self.age = age     # goes through Typed.__set__

    def __repr__(self):
        return f"Person(name={self.name!r}, age={self.age!r})"


# ----------------------------------------------------------
# NOTE: property is ALSO a descriptor
# ----------------------------------------------------------
# The built-in 'property' class implements __get__, __set__, __delete__.
# So @property is actually a convenience wrapper around a descriptor.
#
# class MyClass:
#     @property
#     def x(self): ...
#
# is equivalent to creating a descriptor instance and assigning it to 'x'
# at the class level.


# ----------------------------------------------------------
# Example usage
# ----------------------------------------------------------

if __name__ == "__main__":
    print("=== LoggedAttribute Example ===")
    d1 = DemoLogged()
    d2 = DemoLogged()

    d1.x = 10       # LoggedAttribute.__set__
    d2.x = 99
    print(d1.x)     # LoggedAttribute.__get__
    print(d2.x)

    d1.y = 5
    del d1.y        # LoggedAttribute.__delete__

    print("\n=== PositiveNumber Example ===")
    acc = Account(balance=1000, credit_limit=5000)
    print(acc)

    acc.balance = 1500   # OK
    print(acc)

    # Uncomment to see validation errors:
    # acc.balance = -10           # ValueError
    # acc.credit_limit = "high"   # TypeError

    print("\n=== Typed Descriptor Example ===")
    p = Person("Alice", 30)
    print(p)

    # Uncomment to see type check:
    # p.age = "thirty"  # TypeError
