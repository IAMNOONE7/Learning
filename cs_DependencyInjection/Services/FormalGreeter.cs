using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs_DependencyInjection.Services
{
    // Another IGreeter implementation. More formal tone.
    // Notice: same interface, different behavior. That’s the power of abstraction.
    public sealed class FormalGreeter : IGreeter
    {
        public string Greet(string name) => $"Good day, {name}.";
    }
}
