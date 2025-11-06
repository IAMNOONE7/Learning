using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs_OOP.Contracts
{
    public interface IServiceable
    {
        void PerformService();
        int ServiceIntervalHours { get; }
    }
}
