using cs_Asynchronous.CancellationProgressDemo;
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

namespace cs_Asynchronous.AsyncStreamsDemo
{
    /*
    ======================================================================
    ASYNC STREAMS — IAsyncEnumerable<T>
    ======================================================================
    - Normal IEnumerable<T> gives you ALL items now.
    - IAsyncEnumerable<T> gives you items OVER TIME (asynchronously).
    - Producer can 'yield return' items with 'await' in between.
    - Consumer can use 'await foreach' to read items as they come.

    ----------------------------------------------------------------------
    Producer (async):
    ----------------------------------------------------------------------
    private async IAsyncEnumerable<int> Produce([EnumeratorCancellation] CancellationToken ct)
    {
        for (int i = 1; i <= 10; i++)
        {
            ct.ThrowIfCancellationRequested();
            await Task.Delay(200, ct);  // simulate async work
            yield return i;             // send item to consumer
        }
    }

    ----------------------------------------------------------------------
    Consumer (async):
    ----------------------------------------------------------------------
    await foreach (var item in Produce(ct))
    {
        // process item immediately
    }

    ----------------------------------------------------------------------
    Why it’s useful
    ----------------------------------------------------------------------
    - Streaming logs / telemetry
    - Reading large files line-by-line async
    - Paged API results
    - Device/sensor data over time
    - You don’t have to wait for ALL data to finish to start processing

    ----------------------------------------------------------------------
    Cancellation
    ----------------------------------------------------------------------
    - Add [EnumeratorCancellation] on the token parameter in producer.
    - Call cts.Cancel() from the UI.
    - awaiting 'await foreach' will throw OperationCanceledException -> catch it.

    ======================================================================
    */

    public sealed class AsyncStreamsDemo_ViewModel : INotifyPropertyChanged
    {
        // Items to display in the ListBox
        public ObservableCollection<string> Log { get; } = new();

        private string _statusText = "Ready.";
        public string StatusText
        {
            get => _statusText;
            set { _statusText = value; OnPropertyChanged(); }
        }

        private bool _isStreaming;
        
        // True while we are consuming the async stream.
       
        public bool IsStreaming
        {
            get => _isStreaming;
            set
            {
                if (_isStreaming == value) return;
                _isStreaming = value;
                OnPropertyChanged();
                (StartCommand as AsyncRelayCommand)?.RaiseCanExecuteChanged();
                (CancelCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        // Commands
        public ICommand StartCommand { get; }
        public ICommand CancelCommand { get; }

        // Cancellation for the current streaming session
        private CancellationTokenSource? _cts;

        public AsyncStreamsDemo_ViewModel()
        {
            StartCommand = new AsyncRelayCommand(StartStreamingAsync, () => !IsStreaming);
            CancelCommand = new RelayCommand(Cancel, () => IsStreaming);
        }
       
        // Start consuming the async stream.
        
        private async Task StartStreamingAsync()
        {
            // Create new CTS per run
            _cts = new CancellationTokenSource();

            IsStreaming = true;
            StatusText = "Streaming numbers...";
            Log.Clear();

            try
            {
                // consume the stream
                await foreach (var number in ProduceNumbersAsync(_cts.Token))
                {
                    // We are on the UI context because StartStreamingAsync was awaited from UI
                    // but to be safe, use helper:
                    AddLog($"Received: {number}");
                }

                StatusText = "Stream completed.";
            }
            catch (OperationCanceledException)
            {
                StatusText = "Stream cancelled.";
                AddLog("Stream was cancelled.");
            }
            catch (Exception ex)
            {
                StatusText = "Error: " + ex.Message;
                AddLog("Error: " + ex);
            }
            finally
            {
                _cts?.Dispose();
                _cts = null;
                IsStreaming = false;
            }
        }       
        // Async stream: produces numbers 1..20 with delay.
        // This is IAsyncEnumerable<int> – caller can 'await foreach' it.        
        private async IAsyncEnumerable<int> ProduceNumbersAsync([EnumeratorCancellation] CancellationToken ct)
        {
            // we generate 20 items, 1 per 250 ms
            for (int i = 1; i <= 20; i++)
            {
                ct.ThrowIfCancellationRequested();

                // simulate async work – could be reading from API, device, etc.
                await Task.Delay(250, ct);

                yield return i; // yield single item → caller receives it immediately
            }
        }

        private void Cancel()
        {
            _cts?.Cancel();
        }

        // Safe UI-log helper (in case we move streaming to background later)
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
    // ===== infra: commands + converter =====

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
    public sealed class InverseBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => value is bool b ? !b : Binding.DoNothing;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => value is bool b ? !b : Binding.DoNothing;
    }
}

