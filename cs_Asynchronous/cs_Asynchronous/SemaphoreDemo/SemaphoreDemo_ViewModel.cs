using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace cs_Asynchronous.SemaphoreDemo
{
                /*
            ======================================================================
            SEMAPHORES — EXPLANATION
            ======================================================================
            A semaphore is a counter that limits how many tasks can access a shared 
            resource *at the same time*.

            Think of it as a parking lot with limited spaces:
            - 3 parking spaces → only 3 cars can enter.
            - When one car leaves, another may enter.
            This prevents everyone from rushing in at once.

            In C#, we use:
                SemaphoreSlim semaphore = new SemaphoreSlim(3);

            '3' means allow up to 3 concurrent operations.

            ----------------------------------------------------------------------
            Key methods
            ----------------------------------------------------------------------
            await semaphore.WaitAsync();
                → Asynchronously waits for a free slot.
            semaphore.Release();
                → Frees one slot so another waiting task can start.

            You typically wrap work like this:
                await semaphore.WaitAsync();
                try
                {
                    await DoWorkAsync();
                }
                finally
                {
                    semaphore.Release();
                }

            ----------------------------------------------------------------------
            Real-world use cases
            ----------------------------------------------------------------------
            - Limit concurrent HTTP or API requests (avoid flooding servers)
            - Restrict number of simultaneous database writes
            - Throttle file I/O (too many open files can fail)
            - Control access to a shared device or PLC (one command at a time)

            ----------------------------------------------------------------------
            How it relates to databases
            ----------------------------------------------------------------------
            Databases already handle some concurrency internally, but:
            - If you start hundreds of async inserts at once,
              the connection pool may run out.
            - A SemaphoreSlim(5) can throttle to 5 concurrent inserts.

            Example:
                await _dbLimiter.WaitAsync();
                try { await SaveToDatabaseAsync(); }
                finally { _dbLimiter.Release(); }

            This way, you protect limited resources while still staying async.

            ----------------------------------------------------------------------
            Summary
            ----------------------------------------------------------------------
            - SemaphoreSlim(n): allows *n* tasks to run at the same time.
            - WaitAsync(): waits for permission (non-blocking).
            - Release(): frees permission.
            - Perfect for throttling parallel I/O or DB operations.

            ======================================================================
            */
    public sealed class SemaphoreDemo_ViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<string> Log { get; } = new();
        public ICommand StartCommand { get; }
        public SemaphoreDemo_ViewModel()
        {
            StartCommand = new AsyncRelayCommand(RunDemoAsync);
        }   
        
        // Demonstrates SemaphoreSlim controlling concurrency.
        private async Task RunDemoAsync()
        {
            Log.Clear();
            Log.Add("Starting 10 tasks with limit = 3...");

            // Create a semaphore that allows 3 tasks in parallel.
            using var semaphore = new SemaphoreSlim(3);

            // Create a bunch of tasks (but don't await yet).
            var tasks = new Task[10];
            for (int i = 0; i < tasks.Length; i++)
            {
                int id = i + 1;
                tasks[i] = RunJobAsync(id, semaphore);
            }

            // Wait for all to complete.
            await Task.WhenAll(tasks);

            Log.Add("All tasks finished!");
        }
        
        // One "job" that simulates downloading data.        
        private async Task RunJobAsync(int id, SemaphoreSlim semaphore)
        {
            // WaitAsync() = "ask for permission to enter the parking lot"
            await semaphore.WaitAsync();
            try
            {
                Log.Add($"Job {id} started (slots left: {semaphore.CurrentCount})");
                // Simulate random 1–3s async work
                int delay = Random.Shared.Next(1000, 3000);
                await Task.Delay(delay);

                Log.Add($"Job {id} finished after {delay} ms");
            }
            finally
            {
                // Release() = "free one parking space"
                semaphore.Release();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? prop = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }    
    // Async ICommand implementation for demo.    
    public sealed class AsyncRelayCommand : ICommand
    {
        private readonly Func<Task> _execute;
        private bool _isRunning;

        public AsyncRelayCommand(Func<Task> execute) => _execute = execute;

        public bool CanExecute(object? _) => !_isRunning;

        public async void Execute(object? _)
        {
            if (_isRunning) return;
            _isRunning = true; RaiseCanExecuteChanged();
            try { await _execute(); }
            finally { _isRunning = false; RaiseCanExecuteChanged(); }
        }

        public event EventHandler? CanExecuteChanged;
        private void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
