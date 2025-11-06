# SOLID Principles – E-Shop Order Processing Demo

This simple C# Console Application demonstrates how to apply **five SOLID principles** using a realistic example of an e-shop order workflow.

The goal is to show how proper class design makes the code **clean, testable, and extendable**.

---

## Scenario: Processing an E-Shop Order

When a customer places an order, this app simulates the following steps:

1. The **OrderProcessor** receives the order.
2. It asks multiple **DiscountProviders** (e.g., VIP, Weekend) to calculate discounts.
3. It delegates delivery to a chosen **Delivery method** (Courier or Pickup).
4. It saves the order using a **Repository**.
5. It notifies the customer using a **MessageService** (Email or SMS).

Everything works together through **interfaces and abstractions**, so components can be swapped or extended without touching existing code.

---

## Project Structure

| Folder / File | Description |
|:--|:--|
| `Program.cs` | Entry point — wires everything together (Dependency Injection manually). |
| `Order.cs` | Holds order data (Single Responsibility). |
| `OrderProcessor.cs` | Main workflow coordinator — applies discounts, saves order, sends notifications. |
| `IDiscountProvider.cs`, `CompositeDiscountProvider.cs` | Open/Closed example — multiple discounts combined dynamically. |
| `IDelivery.cs`, `CourierDelivery.cs`, `PickupDelivery.cs` | Liskov Substitution example — all delivery types behave as expected. |
| `IOrderRepository.cs`, `InMemoryOrderRepository.cs` | Interface Segregation example — small, focused storage interface. |
| `IMessageService.cs`, `EmailMessageService.cs`, `SmsMessageService.cs` | Dependency Inversion example — processor depends on abstraction, not implementation. |

---

## SOLID Principles in Action

| Principle | Meaning | Example in Code |
|:--|:--|:--|
| **S** – Single Responsibility | A class should have only one reason to change. | `Order` only stores data; `OrderProcessor` only coordinates. |
| **O** – Open/Closed | Open for extension, closed for modification. | Add new `DiscountProvider` without changing `OrderProcessor`. |
| **L** – Liskov Substitution | Subtypes must be replaceable for their base types. | Any `IDelivery` (Courier, Pickup) can be used interchangeably. |
| **I** – Interface Segregation | Don’t force classes to depend on unused methods. | Small interfaces: `IOrderRepository`, `IMessageService`. |
| **D** – Dependency Inversion | Depend on abstractions, not concrete classes. | `OrderProcessor` uses `IMessageService`, not `EmailMessageService`. |

---

## Real-Life Analogy

| Code Concept | Real-World Equivalent |
|:--|:--|
| `OrderProcessor` | The **manager** coordinating the process. |
| `IDiscountProvider` | The **accountants** who calculate promotions. |
| `IDelivery` | The **courier companies** delivering packages. |
| `IOrderRepository` | The **archive clerk** storing records. |
| `IMessageService` | The **post office** sending notifications. |
| `CompositeDiscountProvider` | The **manager** who asks all discount clerks and sums their results. |

Each employee (class) has one clear job, and the manager doesn’t care how they do it — only that they follow their job description (interface).


