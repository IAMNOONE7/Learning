using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace _02_GC_Basics
{
    /*
        ==================================================================================
        GARBAGE COLLECTION (GC) – NOTES TO MYSELF
        ==================================================================================

        - The GC (Garbage Collector) is like a cleaning robot that frees memory
           from objects I no longer use.

        - GC automatically:
             - finds objects no longer reachable
             - frees their memory on the heap
             - compacts the heap to avoid fragmentation

        - I don’t control when it runs, but I can *force* it manually (only for tests).
           The GC runs when:
             1) The system is low on memory
             2) The heap grows beyond a threshold
             3) I call GC.Collect() manually (rarely needed)

        - Memory in .NET is divided into generations:
             - Gen 0: short-lived objects (e.g., local strings, temp lists)
             - Gen 1: survivors from Gen 0
             - Gen 2: long-lived objects (e.g., global caches, static data)
             - LOH (Large Object Heap): huge arrays or big objects > 85 KB

        - Real-life analogy:
             - Gen 0: today’s coffee cups on my desk — cleaned often
             - Gen 1: things I’m using for a few days — cleaned sometimes
             - Gen 2: things stored permanently on shelves — cleaned rarely

        - Summary:
             - GC helps me not care about freeing memory manually
             - but I must avoid holding references to objects I don’t need
               (because GC won’t collect them if something still points to them)

        ==================================================================================
        | Memory Area   | Who Manages It    | Typical Content                  | Lifetime                  | Speed     |
        | ------------- | ----------------- | -------------------------------- | ------------------------- | --------- |
        | Stack         | Automatic         | Local vars, value types          | Until method returns      | ⚡ Fast   |
        | Heap          | Garbage Collector | Objects (class instances)        | Until no references exist | 🐢 Slower |
        | LOH           | GC (special)      | Large objects (>85KB)            | Until no references exist | 🐢 Slower |
        | Static memory | CLR               | Static/global data               | Until process ends        | —         |
        | Pinned memory | Programmer        | Fixed buffers for native interop | Controlled manually       | ⚠️ Risky  |
        
        ==================================================================================
        ----------------------------------------------------------------------------------
         STACK – fast, short-term memory
        ----------------------------------------------------------------------------------

        - Stack = temporary workspace for methods.
        - Each time I call a method, a stack frame is created with:
          - local variables (like int, double, struct)
          - return address (where to continue after method ends)

        - When the method ends → the stack frame is simply removed.
          That’s super fast — no cleanup or GC needed.

        Example:        
        void DoSomething()
        {
            int x = 10;       // stored directly on stack
            PointStruct p;    // struct fields (X, Y) also on stack
        }
        When DoSomething() finishes, everything inside it is gone automatically.
        ==================================================================================
        ----------------------------------------------------------------------------------
         HEAP – long-term storage managed by the GC
        ----------------------------------------------------------------------------------
        - Heap = a big area of memory where objects (reference types) live.
        - When I create a class instance, C# allocates space on the heap and returns a reference (address).
        var person = new Person();  // stored on heap
        person (the variable) is on the stack,
        but the actual object is on the heap.

        The heap doesn’t automatically clear itself.
        The Garbage Collector (GC) periodically scans for unreachable objects and frees them.
         ----------------------------------------------------------------------------------
        Characteristics:
        - Slower to allocate than stack (because of bookkeeping).
        - Can grow much larger (hundreds of MB or more).
        - Needed for objects that outlive a single method.
        - Managed automatically (but not instantly) by GC.

        Real-life analogy:
        The heap is like a warehouse where I store boxes.
        Each box has an address label (reference).
        As long as I keep the label, the GC won’t touch the box.
        Once no one has the label, the GC eventually comes and removes the box.
        */
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== 02 - Garbage Collection Basics ===\n");

            ShowMemory("Start of program");

            // 1️⃣ Create and destroy some small objects
            Console.WriteLine("Creating temporary objects...");
            for (int i = 0; i < 10000; i++)
            {
                var temp = new byte[1024]; // 1 KB each
                // temp goes out of scope right after this loop iteration
            }

            ShowMemory("After creating 10,000 small byte arrays");

            // 2️⃣ Force the Garbage Collector to run
            Console.WriteLine("\nForcing garbage collection...");
            GC.Collect();                // Forces a full collection
            GC.WaitForPendingFinalizers(); // Wait for finalizers to finish (if any)
            GC.Collect();                // Collect again to ensure full cleanup

            ShowMemory("After GC.Collect()");

            // 3️⃣ Generations demo
            Console.WriteLine("\n=== GC Generations ===");

            var obj1 = new object();
            var obj2 = new byte[50000]; // 50 KB, still in normal heap
            var obj3 = new byte[90000]; // 90 KB, goes to LOH (Large Object Heap)

            Console.WriteLine($"obj1 is in generation: {GC.GetGeneration(obj1)}");
            Console.WriteLine($"obj2 is in generation: {GC.GetGeneration(obj2)}");
            Console.WriteLine($"obj3 is in generation: {GC.GetGeneration(obj3)} (LOH)");

            // 4️⃣ Promote an object by keeping it alive
            Console.WriteLine("\nKeeping an object alive to see promotion...");
            var survivor = new byte[1024];
            for (int i = 0; i < 5; i++)
            {
                GC.Collect();
                Console.WriteLine($"GC cycle {i + 1}, survivor generation: {GC.GetGeneration(survivor)}");
                Thread.Sleep(100);
            }

            // 5️⃣ Show total memory usage again
            ShowMemory("End of program");

            Console.WriteLine("\nDone. Press any key...");
            Console.ReadKey();
        }

        static void ShowMemory(string label)
        {
            long total = GC.GetTotalMemory(false);
            Console.WriteLine($"[{label}] Total managed memory: {total / 1024:N0} KB");
        }
    }
}
