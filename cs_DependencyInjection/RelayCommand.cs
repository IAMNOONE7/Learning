using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace cs_DependencyInjection
{
    // ICommand = a “universal remote control button” interface in WPF.
    // It tells the UI two things:
    // 1) Can I be pressed right now?  (CanExecute)
    // 2) What happens when I’m pressed? (Execute)
    //
    // The ViewModel provides the logic, and the XAML button simply *binds* to this command.
    // So instead of the button knowing "who" it calls, it just says: “When pressed, use this command.”
    public sealed class RelayCommand : ICommand
    {
        // This delegate answers: “Is the button allowed to work right now?”
        // Think of it as the safety lock on a machine — true = allowed, false = locked.
        private readonly Predicate<object?>? _canExecute;

        // This delegate answers: “What should happen when the button is pressed?”
        // Think of it as the actual *action* the remote control triggers.
        private readonly Action<object?> _execute;

        // Constructor: here we hand the command the two behaviors:
        // 1. The actual action (Execute)
        // 2. The rule that decides if it can run (CanExecute)
        //
        //   Real life analogy:
        //   Imagine programming a coffee machine button.
        //   - execute = "Start brewing"
        //   - canExecute = "Do we have water and coffee beans?"
        public RelayCommand(Action<object?> execute, Predicate<object?>? canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        // Called by WPF to check if the command can run.
        // Example: should the "Send Email" button be enabled right now?
        public bool CanExecute(object? parameter)
            => _canExecute?.Invoke(parameter) ?? true; // If no rule provided, assume it's allowed.

        // This runs when the user actually clicks the button.
        // Real world: The “brew” button was pressed — do the thing!
        public void Execute(object? parameter) => _execute(parameter);

        // This event notifies WPF: “Hey, the rules might have changed. Recheck CanExecute()!”
        // Example: maybe the water tank is full now, so the button should become active.
        public event EventHandler? CanExecuteChanged;

        // Manually raise the event (optional). Usually I call this when some condition changes.
        public void RaiseCanExecuteChanged()
            => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
