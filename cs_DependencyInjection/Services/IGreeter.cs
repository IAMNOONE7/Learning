using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs_DependencyInjection.Services
{
    // Interface = just a "contract" that says what methods exist.
    // In this case, any class that implements IGreeter must have a Greet(string name) method.
    public interface IGreeter
    {
        string Greet(string name);
    }
}
