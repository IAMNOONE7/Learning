using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs_DotNetCoreFundamentals.Services
{
    // Abstraction over DateTime.Now so we can swap it in tests or in other environments.
    public interface IDateTimeProvider
    {
        DateTime Now { get; }
    }
}
