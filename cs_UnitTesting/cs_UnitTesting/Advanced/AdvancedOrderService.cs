using cs_UnitTesting.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs_UnitTesting.Advanced
{
    // This service is “more real” — it DOESN’T know discounts itself,
    // it asks a repository. That’s what we’ll mock in tests.
    public class AdvancedOrderService
    {
        private readonly ICustomerRepository _customerRepository;

        public AdvancedOrderService(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public decimal CalculateTotal(string customerId, Order order)
        {
            if (string.IsNullOrWhiteSpace(customerId))
                throw new ArgumentException("customerId is required", nameof(customerId));

            if (order == null)
                throw new ArgumentNullException(nameof(order));

            if (order.Items == null || !order.Items.Any())
                throw new ArgumentException("Order must contain items.");

            // 1) check if customer is allowed to buy
            if (_customerRepository.IsCustomerBlocked(customerId))
                throw new InvalidOperationException("Customer is blocked.");

            // 2) get discount from repo (e.g. DB says this customer has 12%)
            decimal discount = _customerRepository.GetCustomerDiscount(customerId);

            decimal baseTotal = order.Items.Sum(i => i.UnitPrice * i.Quantity);
            decimal finalTotal = baseTotal - (baseTotal * discount);

            return Math.Round(finalTotal, 2);
        }
    }
}
