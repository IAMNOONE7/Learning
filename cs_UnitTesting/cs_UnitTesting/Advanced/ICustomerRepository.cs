using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs_UnitTesting.Advanced
{
    // In real life this would go to DB / API
    public interface ICustomerRepository
    {
       
        // Returns discount (0.0 - 1.0) for a given customerId from data source.
        // Example: 0.15m = 15% discount.       
        decimal GetCustomerDiscount(string customerId);
        
        // Example of another thing repos often do — tell us if customer is blocked.
        // Good for testing branches.        
        bool IsCustomerBlocked(string customerId);
    }
}
