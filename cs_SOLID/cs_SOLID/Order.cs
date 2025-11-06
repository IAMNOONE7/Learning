using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs_SOLID
{
    // SRP: This class is only about representing an Order
    // It does NOT send emails, it does NOT save itself, it does NOT calculate discounts.
    // It's like a paper invoice – it just holds data.
    public class Order
    {
        public int CustomerId { get; }
        public string CustomerName { get; }
        public string CustomerEmail { get; }
        public decimal TotalAmount { get; private set; }
        public IDelivery Delivery { get; }

        public Order(int customerId, string customerName, string customerEmail, decimal totalAmount, IDelivery delivery)
        {
            CustomerId = customerId;
            CustomerName = customerName;
            CustomerEmail = customerEmail;
            TotalAmount = totalAmount;
            Delivery = delivery;
        }

        public void ApplyDiscount(decimal discountAmount)
        {
            // simple case: just reduce amount
            TotalAmount -= discountAmount;
            if (TotalAmount < 0)
                TotalAmount = 0;
        }
    }
}
