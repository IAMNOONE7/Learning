using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs_DependencyInjection.MVVM.ViewModel
{
    // A simple screen — maybe your “dashboard” or “home page”.
    public sealed class HomeViewModel
    {
        // This could later contain logic for the home screen.
        // For now, just a small text to show on screen.
        public string Title => "Welcome to the Home Screen!";
    }
}
