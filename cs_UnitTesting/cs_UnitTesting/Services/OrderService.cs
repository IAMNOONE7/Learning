using cs_UnitTesting.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs_UnitTesting.Services
{
    public class OrderService : IOrderService
    {
        public decimal CalculateTotal(Order order)
        {
            // Basic validation — this is what we want to test too
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            if (order.Items == null || !order.Items.Any())
                throw new ArgumentException("Order must contain at least one item.");

            if (order.Items.Any(i => i.Quantity <= 0))
                throw new ArgumentException("All items must have quantity > 0.");

            // 1) Base total
            decimal baseTotal = order.Items.Sum(i => i.UnitPrice * i.Quantity);

            // 2) Discount by customer type
            decimal discountPercent = order.CustomerType switch
            {
                "Vip" => 0.10m,
                "Employee" => 0.30m,
                _ => 0.00m
            };

            decimal discountAmount = baseTotal * discountPercent;
            decimal finalTotal = baseTotal - discountAmount;

            // Round to 2 decimals like real invoices
            return Math.Round(finalTotal, 2);
        }
    }
}
