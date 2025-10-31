using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace cs_DependencyInjection
{
    // This class holds info about which "screen" (ViewModel) is currently displayed.
    // When CurrentViewModel changes, WPF UI will update automatically (thanks to data binding).
    public sealed class NavigationStore : INotifyPropertyChanged
    {
        // This is the current screen being shown — could be HomeViewModel, SettingsViewModel, etc.
        private object? _currentViewModel;

        // Property used for data binding in MainWindow.
        // When I set this property, I trigger OnPropertyChanged() to refresh the UI.
        public object? CurrentViewModel
        {
            get => _currentViewModel;
            set
            {
                _currentViewModel = value;
                OnPropertyChanged();
            }
        }

        // Event that tells WPF: “Hey, something changed — please re-render this binding.”
        public event PropertyChangedEventHandler? PropertyChanged;

        // Helper to raise the event (the [CallerMemberName] attribute means I don’t have to write the property name manually).
        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
