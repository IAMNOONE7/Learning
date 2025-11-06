using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs_SOLID
{
    // This is our "use case" class.
    // It orchestrates the process, but it DOES NOT do concrete things like "how to send email" or "how to calculate discount".
    // Think of it like a manager in a company – tells others what to do, but doesn't deliver the package himself.
    public class OrderProcessor
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IDiscountProvider _discountProvider;
        private readonly IMessageService _messageService;

        public OrderProcessor(
            IOrderRepository orderRepository,
            IDiscountProvider discountProvider,
            IMessageService messageService)
        {
            _orderRepository = orderRepository;
            _discountProvider = discountProvider;
            _messageService = messageService;
        }

        public void Process(Order order)
        {
            Console.WriteLine("=== Processing order ===");

            // 1) calculate discount (OCP – we can add more discount types easily)
            var discount = _discountProvider.CalculateDiscount(order);
            order.ApplyDiscount(discount);
            Console.WriteLine($"Discount applied: {discount} -> final amount: {order.TotalAmount}");

            // 2) "deliver" – LSP: whatever delivery type we give, it should still work
            order.Delivery.Deliver();

            // 3) save order
            _orderRepository.Save(order);
            Console.WriteLine("Order saved.");

            // 4) notify customer (DIP: we depend on abstraction, not Email directly)
            _messageService.Send(order.CustomerEmail, $"Your order was processed. Final amount: {order.TotalAmount}");

            Console.WriteLine("=== Order finished ===");
        }
    }
}
