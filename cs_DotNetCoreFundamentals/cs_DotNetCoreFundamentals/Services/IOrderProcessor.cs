using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs_DotNetCoreFundamentals.Services
{
    // Contract for our "business logic" part.
    // We keep it as interface so the Program doesn't depend on details.
    public interface IOrderProcessor
    {
        Task ProcessAsync();
    }
}
