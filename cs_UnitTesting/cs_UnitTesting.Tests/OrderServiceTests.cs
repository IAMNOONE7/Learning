using cs_UnitTesting.Models;
using cs_UnitTesting.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace cs_UnitTesting.Tests
{
    /*
    UNIT TESTING
    ==================
    - Unit test = automatic check that verifies a small piece of code (usually one method).
    - Goal: prove that our business logic (like discounts for VIPs) works AND stays working after refactors.
    - We follow the AAA pattern:
        1) ARRANGE - prepare data, create Order, create service
        2) ACT     - call the method under test (CalculateTotal)
        3) ASSERT  - verify the result (is it 900? did it throw?)
    - We test both:
        - Happy path (valid order, valid customer)
        - Edge cases (empty order, null order, bad quantity)
    - We’re using xUnit:
        - [Fact]   -> single test, no parameters
        - [Theory] -> same test with multiple inputs
        - [InlineData] -> defines those inputs
    - Assertions:
        - Assert.Equal(expected, actual) for numbers
        - Assert.Throws<ExceptionType>(() => ...) for errors
    - Good tests are:
        - deterministic (same result every time)
        - fast
        - independent (one test doesn’t depend on another)
    */
    public class OrderServiceTests
    {
        // This is the System Under Test (SUT)
        private readonly OrderService _sut;

        public OrderServiceTests()
        {
            _sut = new OrderService();
        }

        [Fact]
        public void CalculateTotal_RegularCustomer_NoDiscount()
        {
            // ARRANGE
            var order = new Order
            {
                CustomerType = "Regular",
                Items = new List<OrderItem>
                {
                    new OrderItem { ProductName = "Motor", UnitPrice = 100m, Quantity = 2 }, // 200
                    new OrderItem { ProductName = "Sensor", UnitPrice = 50m, Quantity = 1 }  // 50
                }
            };
            // expected base total = 250

            // ACT
            var total = _sut.CalculateTotal(order);

            // ASSERT
            Assert.Equal(250m, total);
        }

        [Fact]
        public void CalculateTotal_VipCustomer_10PercentDiscount()
        {
            // ARRANGE
            var order = new Order
            {
                CustomerType = "Vip",
                Items = new List<OrderItem>
                {
                    new OrderItem { ProductName = "Panel", UnitPrice = 1000m, Quantity = 1 }
                }
            };
            // base = 1000
            // vip discount 10% = 100 → final 900

            // ACT
            var total = _sut.CalculateTotal(order);

            // ASSERT
            Assert.Equal(900m, total);
        }

        [Fact]
        public void CalculateTotal_Employee_30PercentDiscount()
        {
            // ARRANGE
            var order = new Order
            {
                CustomerType = "Employee",
                Items = new List<OrderItem>
                {
                    new OrderItem { ProductName = "PLC", UnitPrice = 2000m, Quantity = 1 }
                }
            };
            // base 2000
            // 30% = 600
            // final 1400

            // ACT
            var total = _sut.CalculateTotal(order);

            // ASSERT
            Assert.Equal(1400m, total);
        }

        [Fact]
        public void CalculateTotal_NullOrder_Throws()
        {
            // ARRANGE
            Order? order = null;

            // ACT + ASSERT
            Assert.Throws<ArgumentNullException>(() => _sut.CalculateTotal(order));
        }

        [Fact]
        public void CalculateTotal_EmptyOrder_Throws()
        {
            // ARRANGE
            var order = new Order
            {
                CustomerType = "Regular",
                Items = new List<OrderItem>() // empty
            };

            // ACT + ASSERT
            var ex = Assert.Throws<ArgumentException>(() => _sut.CalculateTotal(order));
            Assert.Equal("Order must contain at least one item.", ex.Message);
        }

        [Fact]
        public void CalculateTotal_ItemWithZeroQuantity_Throws()
        {
            // ARRANGE
            var order = new Order
            {
                CustomerType = "Regular",
                Items = new List<OrderItem>
                {
                    new OrderItem { ProductName = "Drive", UnitPrice = 500m, Quantity = 0 }
                }
            };

            // ACT + ASSERT
            Assert.Throws<ArgumentException>(() => _sut.CalculateTotal(order));
        }

        // Example of parameterized test
        [Theory]
        [InlineData("Regular", 100, 1, 100)]
        [InlineData("Vip", 100, 1, 90)]
        [InlineData("Employee", 100, 1, 70)]
        public void CalculateTotal_DiscountScenarios_Work(
            string customerType,
            decimal unitPrice,
            int quantity,
            decimal expectedTotal)
        {
            // ARRANGE
            var order = new Order
            {
                CustomerType = customerType,
                Items = new List<OrderItem>
                {
                    new OrderItem { ProductName = "Item", UnitPrice = unitPrice, Quantity = quantity }
                }
            };

            // ACT
            var total = _sut.CalculateTotal(order);

            // ASSERT
            Assert.Equal(expectedTotal, total);
        }
    }
}
