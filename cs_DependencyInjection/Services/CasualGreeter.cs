using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs_DependencyInjection.Services
{
    // This class implements IGreeter. It's one possible version of a greeter.
    // The greeting style is casual.
    public sealed class CasualGreeter : IGreeter
    {
        public string Greet(string name) => $"Hey {name}!";
    }
}
