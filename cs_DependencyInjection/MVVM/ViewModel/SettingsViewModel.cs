using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs_DependencyInjection.MVVM.ViewModel
{
    // Another simple screen — maybe where I adjust settings or preferences.
    public sealed class SettingsViewModel
    {
        // This can later hold logic for user settings, theme, etc.
        public string Title => "Settings Screen";
    }
}
