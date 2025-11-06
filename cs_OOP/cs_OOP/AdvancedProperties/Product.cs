using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs_OOP.AdvancedProperties
{
    /*
      Product shows more "complicated" property logic:
       - private backing field
       - validation in set (can't be negative)
       - computed property (PriceWithVat)
       - expression-bodied property (DisplayName)
     */
    public class Product
    {
        private decimal _price;
        public string Code { get; private set; }
        public string Name { get; private set; }

        // Price with validation
        public decimal Price
        {
            get => _price;
            set
            {
                if (value >= _price)
                {                    
                    _price = value;
                }                
            }
        }

        // Expression-bodied read-only property
        public string DisplayName => $"{Code} - {Name}";

        // Computed property based on other property
        public decimal PriceWithVat => Price * 1.21m;

        public Product(string code, string name, decimal price)
        {
            Code = code;
            Name = name;
            Price = price; // will go through setter, so validation still applies
        }
    }
}
