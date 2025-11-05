using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace _03_FinalizersAndDispose
{
    /*
        ==================================================================================
        FINALIZERS & DISPOSABLES
        ==================================================================================

          GC frees memory — not resources.
            → If my object uses external things (file handles, ports, DB connections),
              GC has no idea what to do with them.

          GC cleans managed memory automatically,
          but I’m responsible for cleaning unmanaged stuff (files, streams, native handles).

        ----------------------------------------------------------------------------------
        FINALIZER
        ----------------------------------------------------------------------------------
        - A special method that GC calls before destroying the object.
        - I can use it to release unmanaged resources if Dispose wasn’t called.
        - But: it’s non-deterministic — I don’t know when it will run!

        Analogy:
           - Like waiting for the cleaning crew in your office building.
             They’ll clean your desk eventually… but you don’t know if it’s today or next week.

        ----------------------------------------------------------------------------------
        IDisposable
        ----------------------------------------------------------------------------------
        - Lets me release resources immediately when I’m done using the object.
        - Used with the `using` statement to automatically call Dispose().

        Analogy:
           - Instead of waiting for the cleaning crew (GC),
             I clean my desk myself before going home.

        ----------------------------------------------------------------------------------
        DISPOSE PATTERN
        ----------------------------------------------------------------------------------
        1) Implement IDisposable.
        2) Provide a protected virtual Dispose(bool disposing).
        3) Suppress finalizer if Dispose() was called manually (GC.SuppressFinalize()).
        4) Free managed resources only when disposing == true.

        Why:
           - GC will always clean managed memory,
             but unmanaged handles (like file handles) must be released manually.
        ==================================================================================

        | Scene                            | In Code                          | Real Life Analogy                                                 |
        | -------------------------------- | -------------------------------- | ----------------------------------------------------------------- |
        | You create the object            | `new FileSimulator_Disposable()` | You open a new office and start using equipment                   |
        | You forget to call `Dispose()`   | No manual cleanup                | You leave coffee cups and papers all over the desk                |
        | GC runs and calls `~Finalizer()` | Cleaning crew eventually comes   | The cleaning crew visits at night and cleans your desk            |
        | You use `using (...) {}`         | Dispose() called immediately     | You clean up before leaving the office                            |
        | You call `GC.SuppressFinalize()` | Tell GC not to bother            | You call the cleaning crew and say “All clean, no need to visit.” |
        | `_disposed` flag                 | Marks whether cleanup is done    | Sticky note saying “already cleaned, don’t repeat”                |

        */
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== 03 - Finalizers and IDisposable ===\n");

            // Demo 1️⃣: Finalizer only (slow cleanup)
            Console.WriteLine("Creating an object with FINALIZER only...");
            CreateFinalizerOnly();
            GC.Collect();
            GC.WaitForPendingFinalizers();

            Console.WriteLine("\n--- Now creating an object that IMPLEMENTS IDisposable ---");
            CreateDisposableObject();

            Console.WriteLine("\n--- Done. Press any key... ---");
            Console.ReadKey();
        }
        static void CreateFinalizerOnly()
        {
            var obj = new FileSimulator_FinalizerOnly("FinalizerOnly.txt");
            obj.DoWork();

            // We don't clean up manually.
            // Imagine leaving the office with coffee cups and papers still on your desk.
            obj = null;

            Console.WriteLine("Object is out of scope. Forcing GC...");
            // Now we wait for the “cleaning crew” (GC) to notice and clean up.
        }

        static void CreateDisposableObject()
        {
            // using = automatic cleanup
            // It’s like I clean my own desk before leaving the office.
            using (var obj = new FileSimulator_Disposable("DisposableFile.txt"))
            {
                obj.DoWork();
                // Desk is messy, but I’ll clean it up when I leave the block.
            }

            // Leaving the "using" block automatically calls Dispose().
            Console.WriteLine("Left the using block. Resource should already be closed.");
        }
    }
    // =========================================================================
    //  Example 1: Class with FINALIZER ONLY (no IDisposable)
    // =========================================================================
    class FileSimulator_FinalizerOnly
    {
        private string _filePath;
        private FileStream _fileStream;

        public FileSimulator_FinalizerOnly(string filePath)
        {
            _filePath = filePath;
            _fileStream = File.Create(filePath);
            Console.WriteLine($"[FinalizerOnly] Created file {_filePath}");
        }

        public void DoWork()
        {
            Console.WriteLine($"[FinalizerOnly] Writing to {_filePath}");
            if (_fileStream != null)
                _fileStream.WriteByte(42);
        }

        // FINALIZER (destructor)
        ~FileSimulator_FinalizerOnly()
        {
            // Imagine the night cleaning crew arriving after I already went home.
            Console.WriteLine($"[FinalizerOnly] FINALIZER running for {_filePath}!");

            // They clean the desk (close file), turn off the lamp, etc.
            if (_fileStream != null)
            {
                _fileStream.Close();
                Console.WriteLine($"[FinalizerOnly] File closed by finalizer.");
            }

            // But they came much later — maybe after hours or even the next day.
        }
    }

    // =========================================================================
    //  Example 2: Proper IDisposable pattern (recommended in real projects)
    // =========================================================================
    class FileSimulator_Disposable : IDisposable
    {
        private string _filePath;
        private FileStream _fileStream;
        private bool _disposed = false;

        public FileSimulator_Disposable(string filePath)
        {
            _filePath = filePath;
            _fileStream = File.Create(filePath);
            Console.WriteLine($"[Disposable] Created file {_filePath}");
        }

        public void DoWork()
        {
            if (_disposed)
                throw new ObjectDisposedException("FileSimulator_Disposable");

            Console.WriteLine($"[Disposable] Writing to {_filePath}");
            if (_fileStream != null)
                _fileStream.WriteByte(42);
        }

        // Called by developer (or automatically by 'using')
        public void Dispose()
        {
            // I’m deciding to clean my own desk before leaving.
            Dispose(true);

            // Tell the cleaning crew “don’t bother, I already cleaned”.
            GC.SuppressFinalize(this);
        }

        // Actual cleanup logic
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return; // Desk already cleaned, nothing to do.

            if (disposing)
            {
                // Clean up managed resources (things that have their own Dispose)
                if (_fileStream != null)
                {
                    _fileStream.Dispose(); // close file
                    Console.WriteLine($"[Disposable] File closed via Dispose()");
                    _fileStream = null;
                }
            }

            // Clean unmanaged resources (if any)
            // Example: closing a database handle or releasing COM object.
            // Think of this as turning off lights or locking doors.

            _disposed = true;
        }

        // FINALIZER – as a backup plan if someone forgot to call Dispose()
        ~FileSimulator_Disposable()
        {
            // The cleaning crew comes late and does emergency cleanup.
            Dispose(false);
            Console.WriteLine($"[Disposable] FINALIZER triggered for {_filePath} (fallback cleanup).");
        }
    }
}
