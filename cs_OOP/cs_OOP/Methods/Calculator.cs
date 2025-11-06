using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs_OOP.Methods
{
    /*
      Calculator shows:
       - instance method (needs object)
       - static method (utility)
       - overloading (same name, different params)
     */
    public class Calculator
    {
        // Instance method – works on THIS object
        public int Add(int a, int b)
        {
            return a + b;
        }

        // Static method – no instance needed, good for pure operations
        public static int AddStatic(int a, int b)
        {
            return a + b;
        }

        // Overload
        public double Add(double a, double b)
        {
            return a + b;
        }
    }
}
