using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs_DependencyInjection.Services
{
    // This is just an interface — it defines *what* a Navigation Service can do.
    // It’s like saying: "Any NavigationService must be able to Navigate to a ViewModel type."
    public interface INavigationService
    {
        // Generic method: I can navigate to any type of ViewModel.
        void Navigate<TViewModel>() where TViewModel : class;
    }
}
