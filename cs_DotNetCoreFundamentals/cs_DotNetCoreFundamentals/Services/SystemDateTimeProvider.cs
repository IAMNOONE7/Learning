using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs_DotNetCoreFundamentals.Services
{
    // Real implementation that just returns the current system time.
    public class SystemDateTimeProvider : IDateTimeProvider
    {
        public DateTime Now => DateTime.Now;
    }
}
