using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace _06_LargeObjectsAndArrays
{
        /*
       ==================================================================================
       LARGE OBJECTS & LOH
       ==================================================================================

       What is a large object in .NET?
       - Any single object ~85,000 bytes or bigger (≈ 85 KB)
       - Typical example: big byte[], big double[], large image buffers, big strings

       Where do they go?
       - They go to a special area of the heap called the Large Object Heap (LOH).
       - LOH is separate from the small-object heap.

       Why special?
       - Large objects are expensive to MOVE.
       - Normal GC likes to compact memory (move objects together to remove holes).
       - Moving 5 MB around all the time would be slow.
       - So: large objects are usually not compacted.
       - That means: lots of allocate/free of big things = possible fragmentation.

       Real-life analogy:
       - Small objects = small boxes - easy to rearrange on shelves.
       - Large objects = sofas and fridges - you don’t want to keep moving them.
       - So you put them into a “big stuff room” (LOH) and try not to move them.

       Rule of thumb:
       - If you allocate many big arrays repeatedly, consider:
           - reusing them,
           - pooling them,
           - or avoiding constant reallocation.

       ==================================================================================
       */
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== 06 - Large Objects and Arrays ===\n");

            ShowMemory("Start");

            // 1️) Allocate a bunch of small arrays (these stay in normal heap)
            Console.WriteLine("Allocating small arrays (not on LOH)...");
            AllocateSmallArrays(2000);
            ShowMemory("After small arrays");

            // 2️) Allocate some LARGE arrays (these go to LOH)
            Console.WriteLine("\nAllocating large arrays (these go to LOH)...");
            var bigOnes = AllocateLargeArrays(20);  // keep references so GC can't free yet
            ShowMemory("After large arrays (LOH)");

            // 3️) Now drop references and force GC
            Console.WriteLine("\nDropping references to large arrays and forcing GC...");
            bigOnes = null;
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            ShowMemory("After GC (large arrays eligible)");

            // 4️) Allocate and free in a loop to simulate fragmentation-ish behavior
            Console.WriteLine("\nAllocating and freeing large arrays in a loop...");
            for (int i = 0; i < 5; i++)
            {
                var temp = AllocateLargeArrays(5);
                ShowMemory($"Loop {i + 1} after allocate");
                temp = null;
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                ShowMemory($"Loop {i + 1} after GC");
                Thread.Sleep(200);
            }

            Console.WriteLine("\nDone. Press any key...");
            Console.ReadKey();
        }

        static void AllocateSmallArrays(int count)
        {
            // These are tiny, GC can compact them easily.
            for (int i = 0; i < count; i++)
            {
                var arr = new byte[1024]; // 1 KB
                // not storing reference -> they can be collected soon
            }
        }

        static List<byte[]> AllocateLargeArrays(int count)
        {
            var list = new List<byte[]>();
            for (int i = 0; i < count; i++)
            {
                // 100,000 bytes = 100 KB → should go to LOH
                // (threshold is ~85 KB)
                var big = new byte[100_000];
                list.Add(big);
                Console.WriteLine($"  Allocated large array #{i + 1} (100 KB)");
            }
            return list;
        }

        static void ShowMemory(string label)
        {
            long total = GC.GetTotalMemory(false);
            Console.WriteLine($"[{label}] Total managed memory: {total / 1024:N0} KB");
        }
    }
}
