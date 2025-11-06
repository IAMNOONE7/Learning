using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs_SOLID
{
    // Abstraction for discount logic
    public interface IDiscountProvider
    {
        decimal CalculateDiscount(Order order);
    }

    // Example 1: VIP discount
    public class VipDiscountProvider : IDiscountProvider
    {
        public decimal CalculateDiscount(Order order)
        {
            // real-life comment:
            // Imagine: "if customer is VIP in our database, we give them 10% off"
            // Here we'll simulate: customerId == 1 is VIP.
            if (order.CustomerId == 1)
            {
                return order.TotalAmount * 0.10m; // 10% discount
            }

            return 0;
        }
    }

    // Example 2: weekend discount
    public class WeekendDiscountProvider : IDiscountProvider
    {
        public decimal CalculateDiscount(Order order)
        {
            // e-shop does weekend sale
            if (DateTime.Now.DayOfWeek == DayOfWeek.Saturday ||
                DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
            {
                return order.TotalAmount * 0.05m; // 5% discount
            }

            return 0;
        }
    }

    // This class combines multiple discount providers.
    // OCP: we can add new provider without changing OrderProcessor.
    public class CompositeDiscountProvider : IDiscountProvider
    {
        private readonly System.Collections.Generic.IEnumerable<IDiscountProvider> _providers;

        public CompositeDiscountProvider(System.Collections.Generic.IEnumerable<IDiscountProvider> providers)
        {
            _providers = providers;
        }

        public decimal CalculateDiscount(Order order)
        {
            decimal totalDiscount = 0;
            foreach (var provider in _providers)
            {
                totalDiscount += provider.CalculateDiscount(order);
            }
            return totalDiscount;
        }
    }
}
