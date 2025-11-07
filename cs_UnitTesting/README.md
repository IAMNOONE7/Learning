# cs_UnitTesting

This project demonstrates **Unit Testing in C#** using the **xUnit** framework.  
It starts with simple tests for a service that calculates order totals,  
then advances to mocking dependencies with **Moq**, similar to what you’d see in real-world codebases.

---
## What’s Covered

### Basic Unit Tests
Demonstrates core xUnit functionality:
- `[Fact]` – single test  
- `[Theory]` + `[InlineData]` – parameterized test  
- `Assert.Equal`, `Assert.Throws`  
- Follows **AAA pattern** (Arrange → Act → Assert)  
- Tests both normal and edge cases

**Service tested:**  
`OrderService` – calculates total price and applies discounts by customer type  
(`Regular`, `Vip`, `Employee`).

---

### Advanced Unit Tests
Demonstrates testing **services with dependencies** using **Moq**.

**Service tested:**  
`AdvancedOrderService` – depends on `ICustomerRepository` to:
- Get discount per customer  
- Check if customer is blocked  

**Test concepts shown:**
- Mocking interfaces  
- Controlling return values with `.Setup()`  
- Verifying calls with `.Verify()`  
- Simulating exception cases  
- Ensuring business logic works without hitting a real database

---

## How to Run Tests

### In Visual Studio
1. Build the solution (`Ctrl + Shift + B`)
2. Open **Test Explorer** (`Test → Test Explorer`)
3. Click **Run All Tests**

Green check marks mean all tests passed  
Red mark indicates a failed test (hover to see reason)