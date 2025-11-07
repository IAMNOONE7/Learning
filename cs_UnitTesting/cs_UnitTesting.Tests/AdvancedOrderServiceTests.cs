using cs_UnitTesting.Advanced;
using cs_UnitTesting.Models;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs_UnitTesting.Tests
{
    /*
    ADVANCED UNIT TESTING
    ===========================
    - In real applications services depend on repositories, APIs, message buses...
      We don't want to call real DBs in unit tests - slow, flaky, needs setup.
    - So we MOCK the dependency.
    - We're using Moq:
        var mock = new Mock<IMyInterface>();
        mock.Setup(x => x.Method(param)).Returns(value);
        var sut = new MyService(mock.Object);
    - This way we test ONLY our service logic, not the DB.
    - Verify(...) is used to ensure our service called the dependency (good for business rules).
    */
    public class AdvancedOrderServiceTests
    {
        // Example customer
        private const string CustomerId = "CUST-001";

        [Fact]
        public void CalculateTotal_UsesDiscountFromRepository()
        {
            // ARRANGE
            var order = new Order
            {
                Items = new List<OrderItem>
                {
                    new OrderItem { ProductName = "Motor", UnitPrice = 1000m, Quantity = 1 }
                }
            };

            // create mock for ICustomerRepository
            var repoMock = new Moq.Mock<ICustomerRepository>();

            // When service asks: GetCustomerDiscount("CUST-001") -> return 0.20 (20%)
            repoMock
                .Setup(r => r.GetCustomerDiscount(CustomerId))
                .Returns(0.20m);

            // Customer is not blocked
            repoMock
                .Setup(r => r.IsCustomerBlocked(CustomerId))
                .Returns(false);

            var sut = new AdvancedOrderService(repoMock.Object);

            // ACT
            var total = sut.CalculateTotal(CustomerId, order);

            // ASSERT
            Assert.Equal(800m, total); // 1000 - 20% = 800

            // optional: verify that repository was actually called
            repoMock.Verify(r => r.GetCustomerDiscount(CustomerId), Times.Once);
            repoMock.Verify(r => r.IsCustomerBlocked(CustomerId), Times.Once);
        }

        [Fact]
        public void CalculateTotal_BlockedCustomer_Throws()
        {
            // ARRANGE
            var order = new Order
            {
                Items = new List<OrderItem>
                {
                    new OrderItem { ProductName = "Sensor", UnitPrice = 200m, Quantity = 2 }
                }
            };

            var repoMock = new Mock<ICustomerRepository>();

            // Now we simulate blocked customer
            repoMock
                .Setup(r => r.IsCustomerBlocked(CustomerId))
                .Returns(true);

            // discount doesn’t matter now, it should throw before
            var sut = new AdvancedOrderService(repoMock.Object);

            // ACT + ASSERT
            Assert.Throws<System.InvalidOperationException>(
                () => sut.CalculateTotal(CustomerId, order));
        }

        [Fact]
        public void CalculateTotal_NoItems_Throws()
        {
            var order = new Order(); // no items

            var repoMock = new Mock<ICustomerRepository>();
            repoMock.Setup(r => r.IsCustomerBlocked(CustomerId)).Returns(false);
            repoMock.Setup(r => r.GetCustomerDiscount(CustomerId)).Returns(0.1m);

            var sut = new AdvancedOrderService(repoMock.Object);

            Assert.Throws<System.ArgumentException>(() => sut.CalculateTotal(CustomerId, order));
        }
    }
}
