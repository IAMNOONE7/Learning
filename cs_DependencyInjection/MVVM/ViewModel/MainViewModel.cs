using cs_DependencyInjection.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace cs_DependencyInjection.MVVM.ViewModel
{
    // This ViewModel is like a "control center" for the window.
    // The UI (XAML) shows things and collects input,
    // but the ViewModel decides what those inputs *mean*.
    public sealed class MainViewModel : INotifyPropertyChanged
    {
        // The greeter is like an external helper I rely on — for example, an employee who knows how to greet people.
        // Instead of learning how to greet myself, I ask my helper to do it.
        private readonly IGreeter _greeter;

        // I hold a reference to the NavigationStore so I know what ViewModel is being shown.
        public NavigationStore Nav { get; }

        // Commands that trigger screen changes.
        public ICommand GoHome { get; }
        public ICommand GoSettings { get; }

        // The ViewModel *doesn’t* create its own helper — it gets one from the outside.
        // This is dependency injection: “Someone else gives me what I need.”
        // Real world: I’m a receptionist, and the manager gives me the proper greeting script.

        public MainViewModel(IGreeter greeter, NavigationStore nav, INavigationService navService)
        {
            _greeter = greeter;         // store the helper

            Name = "Developer";         // default name just for testing

            // Here I tell WPF: “When the user presses the button, run my Greet() method.”
            // RelayCommand connects the button click to my method.
            // Real world analogy: I'm wiring the doorbell to ring the bell when pressed.
            GreetCommand = new RelayCommand(_ => Greet());

            Nav = nav;

            // When the user clicks the "Home" button, call navService.Navigate<HomeViewModel>().
            GoHome = new RelayCommand(_ => navService.Navigate<HomeViewModel>());

            // Same for the settings button.
            GoSettings = new RelayCommand(_ => navService.Navigate<SettingsViewModel>());
        }

        // ---------- PROPERTY: Name ----------
        private string _name = "";

        // The person’s name entered in the TextBox.
        // Think of it as an input field on a web form.
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged(); // tells the UI: “The Name changed — update any textboxes showing it.”
            }
        }

        // ---------- PROPERTY: Message ----------
        private string _message = "Click Greet to see DI in action.";

        // The message that appears below the button.
        // Real world: like the output screen of a kiosk showing “Hello, Mark!”
        public string Message
        {
            get => _message;
            set
            {
                _message = value;
                OnPropertyChanged();
            }
        }

        // ---------- COMMAND ----------
        // This is the command the button binds to.
        // In real life: the "Greet" button on my kiosk.
        public ICommand GreetCommand { get; }

        // When the user clicks the button, this method runs.
        // It’s like pressing a doorbell — the internal wiring (RelayCommand) triggers this action.
        private void Greet()
        {
            // Instead of hardcoding what happens, I delegate the actual greeting to the injected IGreeter.
            // So if the manager gives me a different “greeting script” (FormalGreeter vs CasualGreeter),
            // I behave differently — but *my code here doesn’t change*.
            Message = _greeter.Greet(Name);

            // Real-world analogy:
            // - I’m a receptionist (ViewModel).
            // - I greet a visitor (user presses button).
            // - But I use the script (IGreeter) my manager gave me.
            //   Today it might be “Hey Mark!”, tomorrow “Good morning, Mr. Mark.”
        }

        // ---------- INotifyPropertyChanged ----------
        // WPF watches this event to know when to refresh bound controls.
        public event PropertyChangedEventHandler? PropertyChanged;

        // Helper method that raises PropertyChanged event.
        // [CallerMemberName] auto-fills the name of the property that called this method.
        private void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
