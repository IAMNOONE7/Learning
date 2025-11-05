using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _05_MemoryLeakInDotNet
{
        /*
        ==================================================================================
        MEMORY LEAKS IN .NET
        ==================================================================================
        - A memory leak = when an object stays in memory even though I don't need it anymore,
          because **something still references it.
        - GC can only clean objects that are no longer reachable.
        - If I keep a reference somewhere (static list, event handler, singleton, etc.),
          GC thinks I still need it - so it stays forever.

        Real life:
           - Imagine my office cleaner only removes things that aren’t on my desk.
           - If I keep old papers piled up on the desk corner (even if I never read them),
             the cleaner will never throw them away.
           - The desk keeps getting messier and messier - that’s a memory leak.

        ==================================================================================
        Common causes in .NET
        ----------------------------------------------------------------------------------
        1️) Static collections (e.g., List<>, Dictionary<>) holding references forever.
        2️) Events where subscriber is not unsubscribed.
        3️) Long-living objects referencing short-living ones (e.g., singletons).
        4️) Caches that never clear.
        5️) Closures or lambdas capturing variables unintentionally.

        ==================================================================================
        In this example:
        - We'll make a static list (simulating a global cache)
        - We’ll create many objects and forget to remove them
        - GC can’t clean them, because the static list still holds references
        - Then we’ll fix it to show the proper cleanup
        ==================================================================================
        */
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== 05 - Memory Leaks in .NET ===\n");

            Console.WriteLine("1️) Creating 1000 'leaked' workers (they stay referenced by static list)");
            for (int i = 0; i < 1000; i++)
            {
                var worker = new LeakyWorker($"Worker_{i}");
                worker.StartWork();
            }

            Console.WriteLine("\nCurrent memory (before GC):");
            ShowMemory();

            Console.WriteLine("\nForcing GC (but objects are still referenced by static list):");
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            ShowMemory();

            Console.WriteLine("\nNow fixing the leak by clearing the static list...");
            LeakyWorker.ClearAll();

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            Console.WriteLine("\nAfter clearing static list:");
            ShowMemory();

            Console.WriteLine("\nLesson: If GC can’t see an object as 'unreachable', it will NEVER collect it.");
            Console.WriteLine("Press any key...");
            Console.ReadKey();
        }

        static void ShowMemory()
        {
            long mem = GC.GetTotalMemory(false);
            Console.WriteLine($"Total managed memory: {mem / 1024:N0} KB");
        }
    }

    // =========================================================================
    // Simulated leaking class
    // =========================================================================
    class LeakyWorker
    {
        // Static list = global "memory locker"
        private static List<LeakyWorker> _allWorkers = new List<LeakyWorker>();

        private string _name;

        // add a big buffer so it actually eats memory
        private byte[] _data;

        public LeakyWorker(string name)
        {
            _name = name;
            Console.WriteLine($"[Created] {_name}");
            _data = new byte[1024 * 50]; // 50 KB per worker

            // Each time we create one, we also add it to the static list
            _allWorkers.Add(this);
        }

        public void StartWork()
        {
            // Do something trivial
            Console.WriteLine($"[{_name}] Working...");

            // When this method ends, the object SHOULD be collectible.
            // But because we added it to _allWorkers, it’s still referenced.
        }

        // Optional finalizer for debugging (so we can see if object is collected)
        ~LeakyWorker()
        {
            Console.WriteLine($"[Finalized] {_name} (collected by GC)");
        }

        public static void ClearAll()
        {
            Console.WriteLine("[Static List] Clearing all workers...");
            _allWorkers.Clear();
        }
    }
}
