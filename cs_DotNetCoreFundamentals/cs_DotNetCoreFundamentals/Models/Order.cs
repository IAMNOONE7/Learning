using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs_DotNetCoreFundamentals.Models
{
    // Simple demo model that represents an order.
    // In a real app this could come from a database or an API.
    public class Order
    {
        public int Id { get; set; }
        public string Customer { get; set; }
        public decimal Total { get; set; }
    }
}
