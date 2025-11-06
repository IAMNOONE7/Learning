using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs_SOLID
{
    /*
    ===============================================
         SOLID Principles – Real-life Example
         Project: E-Shop Order Processing Demo
    ===============================================

    This small console application tries to demonstrate all 5 SOLID principles
    in a realistic "E-Shop Order Processing" workflow.

    We simulate what happens when a customer places an order:
    ---------------------------------------------------------
    1. Customer makes an order with total amount 1000.
    2. The OrderProcessor orchestrates the whole flow:
       - applies possible discounts (VIP, Weekend, etc.)
       - delivers the order (via courier or pickup)
       - saves it (in memory or to DB)
       - sends a notification (email or SMS)
    3. Each class has a specific job and follows SOLID rules.

    How data flows:

       [Customer creates an Order]
                 ↓
       -------------------------------
       |  OrderProcessor.Process()   |     - The "Manager" of the process.
       -------------------------------
                 ↓
       1️)  OrderProcessor asks IDiscountProvider 
           - “How much discount should this order get?”
              - CompositeDiscountProvider calls each discount type
                (VipDiscountProvider, WeekendDiscountProvider, etc.)
              - This shows OCP: we can add new discounts without touching core logic.

                 ↓
       2️)  OrderProcessor tells IDelivery to deliver the order.
              - Could be CourierDelivery or PickupDelivery.
              - This shows LSP: every delivery behaves correctly
                when used as IDelivery.

                 ↓
       3️)  OrderProcessor saves the order using IOrderRepository.
              - InMemoryOrderRepository simulates saving to a database.
              - This shows ISP: repository only knows how to save/get,
                nothing else.

                 ↓
       4️)  OrderProcessor notifies the customer using IMessageService.
              - Could be EmailMessageService or SmsMessageService.
              - This shows DIP: processor depends on abstraction,
                not on concrete EmailService.

                 ↓
       [Process complete – Order saved, delivered, customer notified]


    --------------------------------------------------------------
    PRINCIPLES EXPLAINED
    --------------------------------------------------------------

    1️) S – Single Responsibility Principle
       - Each class has ONE job:
         • Order - holds data only
         • OrderProcessor - orchestrates
         • InMemoryOrderRepository - stores orders
         • DiscountProviders - calculate discounts
         • MessageService - sends notifications
       - Real life: like in a company, one person does one role.
         The accountant doesn’t deliver packages.

    2️) O – Open/Closed Principle
       - You can add new discount types without editing OrderProcessor.
         Just implement IDiscountProvider.
       - Example: add BlackFridayDiscountProvider and plug it in.
       - Real life: like adding a new employee role without rewriting company policy.

    3️) L – Liskov Substitution Principle
       - Any IDelivery type (CourierDelivery, PickupDelivery)
         must be usable wherever IDelivery is expected.
       - Real life: whether you send a letter by Post or DHL,
         the recipient still gets it the same way.

    4️) I – Interface Segregation Principle
       - Small focused interfaces: IOrderRepository, IMessageService, IDiscountProvider.
       - Clients depend only on what they need.
       - Real life: a cook doesn’t have to know how to drive the delivery truck.

    5️) D – Dependency Inversion Principle
       - High-level logic (OrderProcessor) depends on abstractions,
         not on concrete classes.
       - Example: OrderProcessor doesn’t know if message is sent by email or SMS.
       - Real life: manager doesn’t care which courier company is used,
         only that the package is delivered.

    --------------------------------------------------------------
    ANALOGY SUMMARY
    --------------------------------------------------------------
    - Order - a paper form with customer and items.
    - DiscountProviders - accountants checking which discounts apply.
    - Delivery - courier services delivering the package.
    - Repository - storage room or database where orders are archived.
    - MessageService - post office or phone system sending notifications.
    - OrderProcessor - the manager who coordinates all of the above.

    --------------------------------------------------------------
    KEY TAKEAWAY
    --------------------------------------------------------------
    SOLID = code that is:
    - easy to understand
    - easy to extend
    - easy to test
    - easy to maintain

    Each change (like adding new discount or notification type)
    requires ZERO modifications to existing code.
    Only new classes plugged into existing abstractions.

    ===============================================
    */

    internal class Program
    {
        static void Main(string[] args)
        {
            // ================================
            // This is our "composition root"
            // Here we decide which implementations we use.
            // In real app we'd use DI container.
            // ================================
            //IMessageService messageService = new EmailMessageService(); // we can later swap to SmsMessageService
            IMessageService messageService = new SmsMessageService();
            IOrderRepository orderRepository = new InMemoryOrderRepository();
            IDiscountProvider discountProvider = new CompositeDiscountProvider(new List<IDiscountProvider>
            {
                new VipDiscountProvider(),
                new WeekendDiscountProvider()
            });

            var orderProcessor = new OrderProcessor(
                orderRepository,
                discountProvider,
                messageService);

            // Create dummy order
            var order = new Order(
                customerId: 1,
                customerName: "John Doe",
                customerEmail: "john.doe@example.com",
                totalAmount: 1000,
                //delivery: new CourierDelivery("123 Street", "DHL")
                delivery: new PickupDelivery("123 Street")
                );

            // Process the order
            orderProcessor.Process(order);

            Console.WriteLine();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
