using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using System.IO;

namespace cs_Asynchronous.LockDemo
{
            /*
            ======================================================================
            LOCK — EXPLANATION
            ======================================================================

            A lock ensures that only one thread can execute a specific section 
            of code at a time. It prevents race conditions — when multiple threads
            try to modify the same variable simultaneously.

            Think of it as a key to a shared coffee machine:
            - Only one person can use the machine at a time.
            - When done, they return the key → next person can use it.

            ----------------------------------------------------------------------
            Basic syntax
            ----------------------------------------------------------------------
            private readonly object _locker = new();

            lock (_locker)
            {
                // Critical section (only one thread at a time)
                SharedCounter++;
            }

            When a thread enters the lock:
            - Other threads trying to enter wait until it’s released.
            - The lock is automatically released when the block ends.

            ----------------------------------------------------------------------
            Without a lock
            ----------------------------------------------------------------------
            _sharedCounter++;
            // Not atomic → multiple threads may read/write at the same time
            // → data corruption (lost increments, inconsistent state)

            With a lock
            lock (_locker)
            {
                _sharedCounter++;
            }
            // Thread-safe, increments always consistent

            ----------------------------------------------------------------------
            Important notes
            ----------------------------------------------------------------------
            - Works only in synchronous code.
            - You cannot use 'await' inside a 'lock' block.
            - Keeps one thread waiting until the other finishes.

            ----------------------------------------------------------------------
            Async equivalent
            ----------------------------------------------------------------------
            In async code, use SemaphoreSlim(1, 1) instead:
                await _mutex.WaitAsync();
                try { await DoWorkAsync(); }
                finally { _mutex.Release(); }
            That acts like an "async lock" — one task at a time, await-friendly.

            ----------------------------------------------------------------------
            Alternatives
            ----------------------------------------------------------------------
            - Interlocked.Increment(ref _sharedCounter);
                → Atomic numeric operations without using lock.
            - ReaderWriterLockSlim
                → Many readers, one writer (sync only).
            - SemaphoreSlim(1,1)
                → Async-friendly exclusive access.

            ----------------------------------------------------------------------          
            Use a lock when:
            ----------------------------------------------------------------------
            - Multiple threads modify the same data in memory.
            - You need to protect critical sections.
            - You want to avoid corrupted state or lost updates.

            ======================================================================
            */
    public sealed class LockDemo_ViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<string> Log { get; } = new();
        private string _result = "Press a button to start.";
        public string Result { get => _result; set { _result = value; OnPropertyChanged(); } }

        public ICommand RunWithoutLockCommand { get; }
        public ICommand RunWithLockCommand { get; }

        public LockDemo_ViewModel()
        {
            RunWithoutLockCommand = new AsyncRelayCommand(RunWithoutLockAsync);
            RunWithLockCommand = new AsyncRelayCommand(RunWithLockAsync);
        }

        // === Example variables ===
        private int _sharedCounter;               // Shared variable accessed by multiple threads
        private readonly object _lockObj = new(); // Used for the lock keyword

        private void AddLog(string message)
        {
            // If we're already on the UI thread, add directly
            if (Application.Current.Dispatcher.CheckAccess())
            {
                Log.Add(message);
            }
            else
            {
                // Otherwise, marshal to UI thread
                Application.Current.Dispatcher.Invoke(() => Log.Add(message));
            }
        }

        // Run example WITHOUT a lock — race condition demonstration.       
        private async Task RunWithoutLockAsync()
        {
            Log.Clear();
            _sharedCounter = 0;
            Result = "Running without lock...";

            int taskCount = 20;      
            int increments = 10_000; 

            var tasks = new Task[taskCount];

            for (int i = 0; i < taskCount; i++)
            {
                int threadId = i + 1;
                tasks[i] = Task.Run(() =>
                {
                    for (int j = 0; j < increments; j++)
                    {
                        // NOT protected
                        _sharedCounter++;
                    }
                    AddLog($"Thread {threadId} done");
                });
            }

            await Task.WhenAll(tasks);

            int expected = taskCount * increments;
            Result = $"Expected {expected:N0} → Actual {_sharedCounter:N0}";
            AddLog("Some increments may be lost due to race conditions (run multiple times to see).");
        }
      
        // Run example WITH a lock — safe increment.        
        private async Task RunWithLockAsync()
        {
            Log.Clear();
            _sharedCounter = 0;
            Result = "Running with lock...";

            int taskCount = 20;
            int increments = 10_000;
            var tasks = new Task[taskCount];

            for (int i = 0; i < taskCount; i++)
            {
                int threadId = i + 1;
                tasks[i] = Task.Run(() =>
                {
                    for (int j = 0; j < increments; j++)
                    {
                        lock (_lockObj)
                        {
                            _sharedCounter++;
                        }
                    }
                    AddLog($"Thread {threadId} done");
                });
            }

            await Task.WhenAll(tasks);

            int expected = taskCount * increments;
            Result = $"Expected {expected:N0} → Actual {_sharedCounter:N0}";
            AddLog("All increments counted correctly — lock prevented the race.");
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        //=======================================================================
        // Just an example
        //=======================================================================
        private readonly SemaphoreSlim _mutex = new(1, 1);

        public async Task WriteToFileAsync(string path, string data)
        {
            await _mutex.WaitAsync();  // await instead of blocking
            try
            {
                // this critical section is async-safe
                await File.AppendAllTextAsync(path, data + "\n");
            }
            finally
            {
                _mutex.Release();
            }
        }
    }

    // Simple async command wrapper
    public sealed class AsyncRelayCommand : ICommand
    {
        private readonly Func<Task> _execute;
        private bool _isRunning;

        public AsyncRelayCommand(Func<Task> execute) => _execute = execute;
        public bool CanExecute(object? _) => !_isRunning;

        public async void Execute(object? _)
        {
            if (_isRunning) return;
            _isRunning = true; Raise();
            try { await _execute(); }
            finally { _isRunning = false; Raise(); }
        }

        public event EventHandler? CanExecuteChanged;
        private void Raise() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
