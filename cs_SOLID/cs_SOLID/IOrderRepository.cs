using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs_SOLID
{
    // Just saving orders – single purpose
    public interface IOrderRepository
    {
        void Save(Order order);
        IEnumerable<Order> GetAll();
    }

    public class InMemoryOrderRepository : IOrderRepository
    {
        private readonly List<Order> _orders = new List<Order>();

        public void Save(Order order)
        {
            // Real life comparison:
            // This is like putting the paper order into a folder.
            _orders.Add(order);
            Console.WriteLine("Order stored in memory (in real app this would be a DB).");
        }

        public IEnumerable<Order> GetAll() => _orders;
    }
}
