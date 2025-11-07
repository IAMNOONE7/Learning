using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs_UnitTesting.Models
{
    public class Order
    {
        public string CustomerType { get; set; } = "Regular"; // Regular, Vip, Employee...
        public List<OrderItem> Items { get; set; } = new();
    }
}
