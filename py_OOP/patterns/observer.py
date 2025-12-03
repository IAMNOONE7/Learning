"""
observer.py
-----------
Observer Pattern in Python.

Idea:
- One object (Subject) maintains a list of dependents (Observers)
- When the Subject changes state, it notifies all observers

Useful for:
- GUI events (button clicked, value changed)
- Logging, monitoring
- Messaging/event systems
- Reacting to stock price changes, sensor changes, etc.
"""

from abc import ABC, abstractmethod
from typing import List, Callable, Protocol, Any


# ----------------------------------------------------------
# EXAMPLE 1: Classic OOP Observer
# ----------------------------------------------------------

class Observer(ABC):
    """
    Abstract observer that reacts to updates from the Subject.
    """

    @abstractmethod
    def update(self, data: Any):
        pass


class Subject:
    """
    Subject keeps track of observers and notifies them.
    """

    def __init__(self):
        self._observers: List[Observer] = []
        self._state: Any = None

    def attach(self, observer: Observer):
        """Subscribe an observer."""
        self._observers.append(observer)

    def detach(self, observer: Observer):
        """Unsubscribe an observer."""
        self._observers.remove(observer)

    def notify(self):
        """Notify all observers about the current state."""
        for obs in self._observers:
            obs.update(self._state)

    def set_state(self, new_state: Any):
        """
        Change state and notify observers.
        In a real system this could be a stock price, sensor value, etc.
        """
        self._state = new_state
        self.notify()


class ConsoleLogger(Observer):
    """
    Observer that logs updates to the console.
    """

    def update(self, data: Any):
        print(f"[ConsoleLogger] New state: {data}")


class AlertSystem(Observer):
    """
    Observer that raises an alert when threshold exceeded.
    """

    def __init__(self, threshold: float):
        self.threshold = threshold

    def update(self, data: Any):
        if isinstance(data, (int, float)) and data > self.threshold:
            print(f"[AlertSystem] ALERT! Value {data} > threshold {self.threshold}")
        else:
            print(f"[AlertSystem] Value {data} OK.")


# ----------------------------------------------------------
# EXAMPLE 2: Pythonic Observer using callbacks
# ----------------------------------------------------------

class ListenerFunc(Protocol):
    """
    A Protocol representing a simple callback function.

    Any callable that takes one argument (data) will match this.
    """

    def __call__(self, data: Any) -> None:
        ...


class Event:
    """
    Very lightweight event system.
    You can:
    - add_listener(func)
    - remove_listener(func)
    - fire(data)
    """

    def __init__(self):
        self._listeners: List[ListenerFunc] = []

    def add_listener(self, listener: ListenerFunc):
        self._listeners.append(listener)

    def remove_listener(self, listener: ListenerFunc):
        self._listeners.remove(listener)

    def fire(self, data: Any):
        for listener in self._listeners:
            listener(data)


# ----------------------------------------------------------
# Example usage
# ----------------------------------------------------------
if __name__ == "__main__":
    print("=== Classic Observer Pattern ===")
    subject = Subject()

    logger = ConsoleLogger()
    alert = AlertSystem(threshold=50)

    subject.attach(logger)
    subject.attach(alert)

    print("\n-- Setting state to 10 --")
    subject.set_state(10)

    print("\n-- Setting state to 75 --")
    subject.set_state(75)

    print("\n-- Detaching logger and setting state to 30 --")
    subject.detach(logger)
    subject.set_state(30)

    print("\n=== Pythonic Event + Callbacks ===")

    def print_listener(data):
        print(f"[print_listener] Got event data: {data}")

    def debug_listener(data):
        print(f"[debug_listener] Debug info: {data!r}")

    price_changed = Event()
    price_changed.add_listener(print_listener)
    price_changed.add_listener(debug_listener)

    print("\n-- Firing event with price 123.45 --")
    price_changed.fire(123.45)

    print("\n-- Removing debug_listener and firing again --")
    price_changed.remove_listener(debug_listener)
    price_changed.fire(999.99)
