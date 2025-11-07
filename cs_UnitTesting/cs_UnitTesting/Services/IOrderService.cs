using cs_UnitTesting.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs_UnitTesting.Services
{
    public interface IOrderService
    {
        // Calculates final total for an order based on items and customer type.
        // Throws if order is invalid.
        decimal CalculateTotal(Order order);
    }
}
