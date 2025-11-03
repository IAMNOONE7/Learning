using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace cs_Asynchronous.CancellationProgressDemo
{

    /*
    ======================================================================
    CANCELLATION TOKEN + PROGRESS REPORTING — EXPLANATION
    ======================================================================    
    Demonstrates how to:
    1. Cancel long-running async operations (user presses Cancel)
    2. Report progress back to the UI (e.g. ProgressBar)

    These are essential for responsive UIs in async code.

    ----------------------------------------------------------------------
    CANCELLATIONTOKEN — HOW IT WORKS
    ----------------------------------------------------------------------
    - A CancellationToken tells an async operation to stop early.
    - It's created using a CancellationTokenSource (CTS):

          var cts = new CancellationTokenSource();
          var token = cts.Token;

    - To cancel:  cts.Cancel();
    - To check for cancel inside async work:
          token.ThrowIfCancellationRequested();

    - If the task is cancelled, it throws OperationCanceledException.
      You can catch it to show "Cancelled by user".

    ----------------------------------------------------------------------
    Example pattern
    ----------------------------------------------------------------------
    private CancellationTokenSource? _cts;

    _cts = new CancellationTokenSource();
    await DoWorkAsync(_cts.Token);

    private async Task DoWorkAsync(CancellationToken ct)
    {
        for (int i = 0; i < 20; i++)
        {
            ct.ThrowIfCancellationRequested(); // check often
            await Task.Delay(200, ct);         // cancel-aware delay
        }
    }

    ----------------------------------------------------------------------
    PROGRESS<T> — HOW IT WORKS
    ----------------------------------------------------------------------
    - Use IProgress<T> and Progress<T> to report progress safely to the UI.

          var progress = new Progress<int>(p => ProgressPercent = p);
          progress.Report(50); // invokes delegate on UI thread automatically

    - Unlike regular delegates, Progress<T> marshals updates
      to the SynchronizationContext (WPF Dispatcher).
      → No need for Dispatcher.Invoke() for UI-bound properties.

    ----------------------------------------------------------------------
    Typical pattern
    ----------------------------------------------------------------------
    private async Task DoLongWorkAsync(IProgress<int> progress, CancellationToken ct)
    {
        int steps = 20;
        for (int i = 1; i <= steps; i++)
        {
            ct.ThrowIfCancellationRequested();
            await Task.Delay(200, ct);
            progress.Report(i * 100 / steps);
        }
    }  
    
    ----------------------------------------------------------------------
    | Concept                  | Purpose / Behavior                          |
    |---------------------------|---------------------------------------------|
    | CancellationTokenSource   | Creates + manages cancellation signals      |
    | CancellationToken         | Passed into async methods to observe cancel |
    | OperationCanceledException| Thrown when cancel is requested             |
    | IProgress<T> / Progress<T>| Reports progress safely to UI thread        |
    | Task.Delay(..., ct)       | Supports cancellation natively              |

    ----------------------------------------------------------------------
    Good practices
    ----------------------------------------------------------------------
     Always create a new CTS for each new operation.
     Dispose CTS after use.
     Check cancellation regularly in loops.
     Catch OperationCanceledException to end gracefully.
     Use Progress<T> for UI-safe updates (auto marshals to UI).
    ======================================================================
    */
    public sealed class CancellationProgress_ViewModel : INotifyPropertyChanged
    {
        // Collection bound to ListBox – we'll log steps here
        public ObservableCollection<string> Log { get; } = new();

        private string _statusText = "Ready.";
        public string StatusText
        {
            get => _statusText;
            set { _statusText = value; OnPropertyChanged(); }
        }

        private int _progressPercent;
        public int ProgressPercent
        {
            get => _progressPercent;
            set { _progressPercent = value; OnPropertyChanged(); }
        }

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                if (_isBusy == value) return;
                _isBusy = value;
                OnPropertyChanged();
                (StartCommand as AsyncRelayCommand)?.RaiseCanExecuteChanged();
                (CancelCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        // Commands
        public ICommand StartCommand { get; }
        public ICommand CancelCommand { get; }

        // Cancellation source – new for each run
        private CancellationTokenSource? _cts;

        public CancellationProgress_ViewModel()
        {
            StartCommand = new AsyncRelayCommand(StartWorkAsync, () => !IsBusy);
            CancelCommand = new RelayCommand(Cancel, () => IsBusy);
        }
       
        // Starts a long-running async operation that reports progress
        // and supports cancellation.
       
        private async Task StartWorkAsync()
        {
            // create new CTS for this run
            _cts = new CancellationTokenSource();

            // Progress object – this automatically posts to UI thread
            var progress = new Progress<int>(p => ProgressPercent = p);

            IsBusy = true;
            ProgressPercent = 0;
            Log.Clear();
            StatusText = "Working...";

            try
            {
                // simulate long work
                await DoLongWorkAsync(progress, _cts.Token);
                StatusText = "Completed successfully.";
                Log.Add("Work finished.");
            }
            catch (OperationCanceledException)
            {
                StatusText = "Cancelled by user.";
                Log.Add("Operation was cancelled.");
            }
            catch (Exception ex)
            {
                StatusText = "Error: " + ex.Message;
                Log.Add(" Error: " + ex);
            }
            finally
            {
                _cts?.Dispose();
                _cts = null;
                IsBusy = false;
            }
        }
        
        // Simulates a long operation made of multiple steps.
        // Each step checks for cancellation and reports progress.
       
        private async Task DoLongWorkAsync(IProgress<int> progress, CancellationToken ct)
        {
            int totalSteps = 20; // 20 steps -> 5% per step

            for (int i = 1; i <= totalSteps; i++)
            {
                // Check cancellation early – this throws OperationCanceledException
                ct.ThrowIfCancellationRequested();

                // Simulate some async I/O or processing
                await Task.Delay(200, ct); // supports cancellation

                // Report progress – will update ProgressBar
                int percent = i * 100 / totalSteps;
                progress.Report(percent);

                AddLog($"Step {i}/{totalSteps} done.");
            }
        }

        private void Cancel()
        {
            _cts?.Cancel();
        }

        // Helper to safely add to Log from UI or background
        private void AddLog(string message)
        {
            if (Application.Current.Dispatcher.CheckAccess())
            {
                Log.Add(message);
            }
            else
            {
                Application.Current.Dispatcher.Invoke(() => Log.Add(message));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? prop = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }

    public sealed class AsyncRelayCommand : ICommand
    {
        private readonly Func<Task> _execute;
        private readonly Func<bool>? _canExecute;
        private bool _isRunning;

        public AsyncRelayCommand(Func<Task> execute, Func<bool>? canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object? _) => !_isRunning && (_canExecute?.Invoke() ?? true);

        public async void Execute(object? _)
        {
            if (!CanExecute(null)) return;
            _isRunning = true; RaiseCanExecuteChanged();
            try { await _execute(); }
            finally { _isRunning = false; RaiseCanExecuteChanged(); }
        }

        public event EventHandler? CanExecuteChanged;
        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }

    public sealed class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool>? _canExecute;
        public RelayCommand(Action execute, Func<bool>? canExecute = null)
        {
            _execute = execute; _canExecute = canExecute;
        }
        public bool CanExecute(object? _) => _canExecute?.Invoke() ?? true;
        public void Execute(object? _) => _execute();
        public event EventHandler? CanExecuteChanged;
        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }

    // Inverse bool converter so we can do IsEnabled = !IsBusy
    public sealed class InverseBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => value is bool b ? !b : Binding.DoNothing;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => value is bool b ? !b : Binding.DoNothing;
    }
}
