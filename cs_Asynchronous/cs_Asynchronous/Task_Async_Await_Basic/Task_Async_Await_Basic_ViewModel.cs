using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;

namespace cs_Asynchronous.Task_Async_Await_Basic
{

             /* 
            ======================================================================
            ASYNC / AWAIT / TASK — EXPLANATION
            ======================================================================
            1. Task — the foundation
            ----------------------------------------------------------------------
            - Task represents an operation that runs asynchronously.
            - It’s like a “promise” that something will finish later.
            - Two main forms:
                Task        → represents a future operation with no return value.
                Task<T>     → represents a future operation that returns a value.

            Example:
                Task DoWorkAsync();
                Task<int> CalculateAsync();

            A Task can be in several states:
                - Running
                - CompletedSuccessfully
                - Faulted (threw an exception)
                - Canceled

            You can await a Task, check its Status, or handle its Exception.

            ----------------------------------------------------------------------
            2. async — marks a method as asynchronous
            ----------------------------------------------------------------------
            - “async” tells the compiler this method may contain 'await' keywords.
            - The compiler transforms it into a state machine.
            - It automatically returns a Task or Task<T>.
            - Without 'async', you cannot use 'await'.

            Example:
                public async Task DoSomethingAsync()
                {
                    await Task.Delay(1000);  // non-blocking delay
                }

            The method executes normally until it hits 'await',
            then it pauses and returns control to the caller.
            When the awaited task finishes, execution resumes
            at the next line.

            ----------------------------------------------------------------------
            3. await — asynchronous waiting
            ----------------------------------------------------------------------
            - 'await' pauses the async method until the awaited Task completes.
            - The key difference: it doesn’t block the current thread.
            - In WPF or WinForms, the method resumes on the UI thread automatically.

            Example:
                private async void Btn_Click(object sender, RoutedEventArgs e)
                {
                    Status.Text = "Starting...";
                    await Task.Delay(2000);     // Wait 2s (UI still responsive)
                    Status.Text = "Done!";      // Runs after 2 seconds
                }

            While waiting, the UI thread is free to repaint, respond to user input, etc.

            ----------------------------------------------------------------------
            4. How async/await/Task fit together
            ----------------------------------------------------------------------
            | Keyword  | Meaning  |
            |----------|----------|
            | Task     | Represents the asynchronous operation itself. |
            | async    | Allows using 'await' inside a method; wraps return into a Task. |
            | await    | Waits for a Task asynchronously, then resumes the method. |

            Example:
                public async Task<int> GetDataAsync()
                {
                    string data = await DownloadAsync();
                    return data.Length;
                }

            Flow:
                - Method starts and calls DownloadAsync().
                - 'await' tells the compiler to suspend until DownloadAsync() completes.
                - The method returns a Task<int> immediately (not the final result yet).
                - When DownloadAsync() finishes, the method resumes and returns the result.        

            ----------------------------------------------------------------------
            5. Common mistakes
            ----------------------------------------------------------------------           

            -  Using 'async void' (except for event handlers)
                → Cannot be awaited; exceptions are harder to catch.
                Use 'async Task' whenever possible.            

            -  Doing heavy CPU work in async method
                → Doesn't make it faster; still runs on one thread.
                Use 'await Task.Run(() => HeavyWork())' for CPU tasks.

            ----------------------------------------------------------------------
            Quick mental model
            ----------------------------------------------------------------------
            - Task  = a "ticket" for work happening in the future.
            - await = "pause here until the ticket finishes, then continue."
            - async = "compiler, please make this method await-friendly."

            ----------------------------------------------------------------------
            Timeline visualization
            ----------------------------------------------------------------------
            async method starts
                ↓
            runs code until first await
                ↓
            await Task.Delay(2000)
              ├── suspends method
              ├── control returns to UI / caller
              └── after 2s, resumes method at next line
                ↓
            method finishes → Task completed

            ======================================================================
            */



    public sealed class Task_Async_Await_Basic_ViewModel : INotifyPropertyChanged
    {
        // === Bindable properties ===

        // Reuse a single HttpClient (best practice).
        private readonly HttpClient _http = new();

        // ===== New: simple log + progress =====
        private int _progressPercent;
        public int ProgressPercent
        {
            get => _progressPercent;
            set { _progressPercent = value; OnPropertyChanged(); }
        }

        public ObservableCollection<string> Log { get; } = new();

        private string _statusText = "Ready."; // What the UI shows under the button
        public string StatusText
        {
            get => _statusText;
            set { _statusText = value; OnPropertyChanged(); } // Notify binding system
        }

        private bool _isBusy; // When true, we disable the start button
        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                if (_isBusy == value) return;
                _isBusy = value;
                OnPropertyChanged();

                (StartCommand as AsyncRelayCommand)?.RaiseCanExecuteChanged();
                (StartParallelCommand as AsyncRelayCommand)?.RaiseCanExecuteChanged();
                (StartBoundedCommand as AsyncRelayCommand)?.RaiseCanExecuteChanged();
                (StartRetryCommand as AsyncRelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        // Exposed command that the Button binds to.
        public ICommand StartCommand { get; }
        public ICommand StartParallelCommand { get; }
        public ICommand StartBoundedCommand { get; }
        public ICommand StartRetryCommand { get; }

        public Task_Async_Await_Basic_ViewModel()
        {
            // AsyncRelayCommand wraps an async method and integrates with ICommand.
            // The CanExecute predicate here says "only when not busy".
            StartCommand = new AsyncRelayCommand(RunSequenceAsync, () => !IsBusy);
            StartParallelCommand = new AsyncRelayCommand(RunParallelAsync, () => !IsBusy);           
            StartRetryCommand = new AsyncRelayCommand(RunRetryAsync, () => !IsBusy);
        }

        
        // Async workflow with multiple awaits in sequence.
        // Each await yields back to the UI thread without blocking it.
        
        private async Task RunSequenceAsync()
        {
            try
            {
                IsBusy = true;                    // Disable button via binding
                StatusText = "Step 1: Preparing…";
                await Task.Delay(1000);           // Simulate async work (I/O later)

                StatusText = "Step 2: Downloading…";
                await Task.Delay(2000);           // Another async step

                StatusText = "Step 3: Saving…";
                await Task.Delay(1500);           // Final async step

                StatusText = "All done!";
            }
            finally
            {
                // Ensure UI re-enables even if an exception occurs
                IsBusy = false;
            }
        }


        // ==============================================================
        // Demo 1: PARALLEL awaits — run several independent tasks at once
        // ==============================================================
        private async Task RunParallelAsync()
        {
            try
            {
                IsBusy = true;
                StatusText = "Parallel: starting downloads…";
                Log.Clear();

                // A few small, reliable pages (use anything you like)
                var urls = new[]
                {
                "https://www.seznam.cz/",
                "https://en.wikipedia.org/wiki/C_Sharp_(programming_language)"                      
            };

                // Kick off all tasks at once (don’t await yet).
                var tasks = urls.Select(GetPageLengthAsync).ToList();

                // Await all together. Fails fast if any throws.
                var lengths = await Task.WhenAll(tasks);

                // Sum the lengths and show result
                int total = lengths.Sum();
                StatusText = $"Parallel: {urls.Length} pages, total {total:N0} chars.";
                foreach (var (u, len) in urls.Zip(lengths))
                    Log.Add($"{u} → {len:N0} chars");
            }
            catch (Exception ex)
            {
                StatusText = "Parallel: error — " + ex.Message;
            }
            finally
            {
                IsBusy = false;
            }
        }

        // Helper: returns the length of a page (Task<int> -> Task<T> demo)
        private async Task<int> GetPageLengthAsync(string url)
        {
            using var resp = await _http.GetAsync(url);

            // Don't throw on 403/404 — just treat it as "0 length"
            if (!resp.IsSuccessStatusCode)
            {
                // you can log this somewhere
                return 0;
            }

            var html = await resp.Content.ReadAsStringAsync();
            return html.Length;
        }

        // ======================================================
        // Demo 2: TIMEOUT + simple RETRY with exponential backoff
        // ======================================================
        private async Task RunRetryAsync()
        {
            try
            {
                IsBusy = true;
                StatusText = "Retry: trying with timeout…";
                Log.Clear();

                string url = "https://httpbin.org/delay/3"; // responds after ~3s

                // We’ll try up to 3 times, each with a 1.5s timeout.
                int attempts = 3;
                TimeSpan perAttemptTimeout = TimeSpan.FromSeconds(1.5);

                int finalLength = await GetWithTimeoutAndRetryAsync(
                    () => GetPageLengthAsync(url),
                    perAttemptTimeout,
                    attempts);

                StatusText = $"Retry: success — length {finalLength:N0} chars.";
            }
            catch (Exception ex)
            {
                StatusText = "Retry: failed — " + ex.Message;
            }
            finally
            {
                IsBusy = false;
            }
        }

        // Utility: run an async func with per-attempt timeout + exponential backoff
        private async Task<T> GetWithTimeoutAndRetryAsync<T>(Func<Task<T>> factory, TimeSpan perAttemptTimeout, int maxAttempts)
        {
            // Simple backoff: 0.5s, 1s, 2s between attempts (after first failure)
            TimeSpan Backoff(int attempt) => TimeSpan.FromMilliseconds(500 * Math.Pow(2, attempt - 1));

            Exception? last = null;
            for (int attempt = 1; attempt <= maxAttempts; attempt++)
            {
                try
                {
                    Log.Add($"Attempt {attempt}… (timeout {perAttemptTimeout.TotalSeconds:0.0}s)");

                    // Run the task with a timeout race
                    var workTask = factory();
                    var timeoutTask = Task.Delay(perAttemptTimeout);

                    var finished = await Task.WhenAny(workTask, timeoutTask);
                    if (finished == timeoutTask)
                        throw new TimeoutException("Operation timed out.");

                    // If we’re here, workTask finished first; unwrap result/exception
                    return await workTask; // will rethrow if faulted
                }
                catch (Exception ex)
                {
                    last = ex;
                    Log.Add($"Attempt {attempt} failed: {ex.Message}");

                    if (attempt == maxAttempts) break; // out of attempts

                    var delay = Backoff(attempt);
                    Log.Add($"Waiting {delay.TotalMilliseconds:0} ms before retry…");
                    await Task.Delay(delay);
                }
            }

            // Exhausted all attempts
            throw new InvalidOperationException("All attempts failed.", last);
        }

        // === INotifyPropertyChanged boilerplate ===
        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? prop = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
   
    // Minimal async ICommand implementation.
    // - Prevents double execution (while running).
    // - Awaits the task to surface exceptions.
    // - Supports CanExecute (e.g., disable button when IsBusy).  
    public sealed class AsyncRelayCommand : ICommand
    {
        private readonly Func<Task> _execute;         // The async body to run
        private readonly Func<bool>? _canExecute;     // Optional predicate
        private bool _isRunning;                      // Prevent re-entrancy

        public AsyncRelayCommand(Func<Task> execute, Func<bool>? canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object? _) => !_isRunning && (_canExecute?.Invoke() ?? true);

        // ICommand.Execute must be void. We still await internally to catch errors.
        public async void Execute(object? _)
        {
            if (!CanExecute(null)) return;

            _isRunning = true;
            RaiseCanExecuteChanged(); // Notify WPF to re-query CanExecute

            try
            {
                await _execute();     // Run the actual async work
            }
            finally
            {
                _isRunning = false;
                RaiseCanExecuteChanged();
            }
        }

        public event EventHandler? CanExecuteChanged;

        public void RaiseCanExecuteChanged()
            => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    } 
    
    // Converts bool to !bool for bindings like IsEnabled = !IsBusy.
    
    public sealed class InverseBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => value is bool b ? !b : Binding.DoNothing;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => value is bool b ? !b : Binding.DoNothing;
    }

}


