"""
strategy.py
-----------
Strategy Pattern in Python.

Idea:
- Define a family of algorithms (strategies)
- Make them interchangeable
- Let the caller choose which strategy to use at runtime

We will implement:
1. Classic OOP Strategy with ABC
2. Using the strategy inside a Context class
3. Pythonic Strategy using simple functions
"""

from abc import ABC, abstractmethod
from typing import Protocol, Callable


# ----------------------------------------------------------
# EXAMPLE 1: Classic OOP Strategy – discount calculation
# ----------------------------------------------------------

class DiscountStrategy(ABC):
    """
    Abstract base class representing a discount strategy.
    """

    @abstractmethod
    def apply_discount(self, price: float) -> float:
        pass


class NoDiscount(DiscountStrategy):
    def apply_discount(self, price: float) -> float:
        return price


class PercentageDiscount(DiscountStrategy):
    def __init__(self, percent: float):
        self.percent = percent

    def apply_discount(self, price: float) -> float:
        return price * (1 - self.percent / 100.0)


class FixedAmountDiscount(DiscountStrategy):
    def __init__(self, amount: float):
        self.amount = amount

    def apply_discount(self, price: float) -> float:
        result = price - self.amount
        return max(result, 0)  # no negative prices


class BlackFridayDiscount(DiscountStrategy):
    def apply_discount(self, price: float) -> float:
        # Just a crazy discount for demo purposes :)
        return price * 0.5


class PriceCalculator:
    """
    Context class that uses a DiscountStrategy.
    It doesn't care *which* strategy is used.
    """

    def __init__(self, strategy: DiscountStrategy):
        self.strategy = strategy

    def set_strategy(self, strategy: DiscountStrategy):
        """Change strategy at runtime."""
        self.strategy = strategy

    def calculate_price(self, base_price: float) -> float:
        return self.strategy.apply_discount(base_price)


# ----------------------------------------------------------
# EXAMPLE 2: Pythonic Strategy – functions instead of classes
# ----------------------------------------------------------

# A Protocol describing our function-based strategy (for type checkers)
class DiscountFunc(Protocol):
    def __call__(self, price: float) -> float:
        ...


def no_discount(price: float) -> float:
    return price


def vip_discount(price: float) -> float:
    return price * 0.8  # 20% off


def student_discount(price: float) -> float:
    return price * 0.9  # 10% off


def special_event_discount(price: float) -> float:
    if price > 100:
        return price - 30  # flat 30 off for big purchases
    return price


class FunctionalPriceCalculator:
    """
    Strategy via callables (functions) instead of classes.
    Very Pythonic and lightweight.
    """

    def __init__(self, strategy: DiscountFunc):
        self.strategy = strategy

    def set_strategy(self, strategy: DiscountFunc):
        self.strategy = strategy

    def calculate_price(self, base_price: float) -> float:
        return self.strategy(base_price)


# ----------------------------------------------------------
# Example usage
# ----------------------------------------------------------
if __name__ == "__main__":
    print("=== Classic Strategy Pattern ===")
    base_price = 200.0

    # Choose strategy at runtime:
    calc = PriceCalculator(NoDiscount())
    print("No discount:           ", calc.calculate_price(base_price))

    calc.set_strategy(PercentageDiscount(10))
    print("10% discount:          ", calc.calculate_price(base_price))

    calc.set_strategy(FixedAmountDiscount(50))
    print("50 off:                ", calc.calculate_price(base_price))

    calc.set_strategy(BlackFridayDiscount())
    print("Black Friday (50% off):", calc.calculate_price(base_price))

    print("\n=== Functional Strategy (Pythonic) ===")
    fcalc = FunctionalPriceCalculator(no_discount)
    print("No discount:           ", fcalc.calculate_price(base_price))

    fcalc.set_strategy(vip_discount)
    print("VIP discount (20% off):", fcalc.calculate_price(base_price))

    fcalc.set_strategy(student_discount)
    print("Student discount (10%):", fcalc.calculate_price(base_price))

    fcalc.set_strategy(special_event_discount)
    print("Special event:         ", fcalc.calculate_price(base_price))
