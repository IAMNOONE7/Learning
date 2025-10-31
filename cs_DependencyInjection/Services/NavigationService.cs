using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs_DependencyInjection.Services
{
    // This class is the actual "navigator" — it knows how to change screens.
    // It uses Dependency Injection (IServiceProvider) to create ViewModels automatically.
    public sealed class NavigationService : INavigationService
    {
        // The store is where we keep track of which screen is currently shown.
        private readonly NavigationStore _store;

        // The service provider (from DI container) is what creates ViewModels for us.
        private readonly IServiceProvider _sp;

        public NavigationService(NavigationStore store, IServiceProvider sp)
        {
            _store = store;
            _sp = sp;
        }

        // Navigate<TViewModel> means “switch to a new ViewModel of type TViewModel”.
        public void Navigate<TViewModel>() where TViewModel : class
        {
            // Ask DI to give me a new instance of that ViewModel (and all its dependencies).
            var viewModel = _sp.GetRequiredService<TViewModel>();

            // Now I just tell the store that this is the new "current screen".
            // WPF will automatically show the matching View (thanks to DataTemplates).
            _store.CurrentViewModel = viewModel;
        }
    }
}
