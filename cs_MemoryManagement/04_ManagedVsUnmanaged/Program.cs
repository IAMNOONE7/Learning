using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace _04_ManagedVsUnmanaged
{
        /*
       ==================================================================================
       MANAGED vs UNMANAGED
       ==================================================================================

       - Managed = things the .NET runtime knows about and can clean up.
         Examples: string, List<>, my own classes, arrays, DateTime...
         - GC can track them.

       - Unmanaged = things OUTSIDE of .NET. OS handles, native memory, files, sockets...
         - GC does NOT know how to release these.
         - I must release them manually (Dispose / Close / FreeHGlobal).

        Real life:
         - Managed: toys in kid’s room — robot (GC) recognizes them and puts them back.
         - Unmanaged: hot pan on stove — robot doesn’t know it’s dangerous, I must put it away myself.

       This example shows:
         1) A fully managed object (GC can handle it)
         2) An object that allocates UNMANAGED memory (GC can’t handle it)
         3) Proper pattern to free unmanaged stuff
       ==================================================================================
       */
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== 04 - Managed vs Unmanaged ===\n");

            Console.WriteLine("1) Managed-only object (GC can handle this):");
            UseManagedObject();

            Console.WriteLine("\n2) Unmanaged-like object WITHOUT proper cleanup (bad):");
            CreateAndForgetUnmanaged();   // we don't dispose it

            Console.WriteLine("\n3) Unmanaged-like object WITH Dispose (good):");
            UseUnmanagedWithDispose();    // we dispose it

            Console.WriteLine("\nForcing GC to show behavior...\n");
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            Console.WriteLine("\n--- Done. Press any key... ---");
            Console.ReadKey();
        }

        static void UseManagedObject()
        {
            // This is pure managed. GC can take care of everything.
            var m = new ManagedHolder();
            m.DoWork();
            // when method ends, GC will collect it when it wants to
        }

        static void CreateAndForgetUnmanaged()
        {
            var u = new UnmanagedHolder_Bad(1024 * 10); // 10 KB of unmanaged memory
            u.DoWork();

            // We "forget" to free it.
            // This is like taking a chair from outside, putting it in the office,
            // and never returning it. Cleaning crew doesn’t know it doesn’t belong here.
        }

        static void UseUnmanagedWithDispose()
        {
            // Using the correct version
            using (var u = new UnmanagedHolder_Good(1024 * 10))
            {
                u.DoWork();
            } // <- Dispose is called here, unmanaged memory is returned
        }
    }

    // =========================================================================
    // 1) Managed-only class – GC can handle this
    // =========================================================================
    class ManagedHolder
    {
        private byte[] _buffer;

        public ManagedHolder()
        {
            // This is a normal managed array on the heap.
            _buffer = new byte[1024];
            Console.WriteLine("[Managed] Allocated managed array of 1 KB.");
        }

        public void DoWork()
        {
            Console.WriteLine("[Managed] Doing some work...");
        }
    }

    // =========================================================================
    // 2) BAD example – class that allocates unmanaged memory but doesn't free it
    // =========================================================================
    class UnmanagedHolder_Bad
    {
        private IntPtr _nativePtr;
        private int _size;

        public UnmanagedHolder_Bad(int size)
        {
            _size = size;

            // This allocates memory OUTSIDE of .NET (OS-level allocation)
            _nativePtr = Marshal.AllocHGlobal(size);
            Console.WriteLine($"[Bad Unmanaged] Allocated {_size} bytes of UNMANAGED memory.");
        }

        public void DoWork()
        {
            Console.WriteLine("[Bad Unmanaged] Using unmanaged memory...");
            // We could write to _nativePtr, but not needed for demo
        }

        ~UnmanagedHolder_Bad()
        {
            // We added a finalizer to at least free it eventually.
            // But if we forget this finalizer - we LEAK unmanaged memory.
            if (_nativePtr != IntPtr.Zero)
            {
                Console.WriteLine("[Bad Unmanaged] FINALIZER freeing unmanaged memory (late).");
                Marshal.FreeHGlobal(_nativePtr);
                _nativePtr = IntPtr.Zero;
            }
        }
    }

    // =========================================================================
    // 3) GOOD example – proper Dispose pattern for unmanaged memory
    // =========================================================================
    class UnmanagedHolder_Good : IDisposable
    {
        private IntPtr _nativePtr;
        private int _size;
        private bool _disposed;

        public UnmanagedHolder_Good(int size)
        {
            _size = size;
            _nativePtr = Marshal.AllocHGlobal(size);
            Console.WriteLine($"[Good Unmanaged] Allocated {_size} bytes of UNMANAGED memory.");
        }

        public void DoWork()
        {
            if (_disposed)
                throw new ObjectDisposedException("UnmanagedHolder_Good");

            Console.WriteLine("[Good Unmanaged] Doing work with unmanaged memory...");
        }

        public void Dispose()
        {
            // I’m explicitly saying “I’m done, clean this now”.
            Dispose(true);
            // Tell GC “don’t run finalizer, I already cleaned”
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            // First: free unmanaged stuff – that’s the main reason this class exists
            if (_nativePtr != IntPtr.Zero)
            {
                Console.WriteLine("[Good Unmanaged] Freeing unmanaged memory in Dispose()");
                Marshal.FreeHGlobal(_nativePtr);
                _nativePtr = IntPtr.Zero;
            }

            // If we also had managed fields that implement IDisposable,
            // we would clean them here if (disposing) { foo.Dispose(); }

            _disposed = true;
        }

        ~UnmanagedHolder_Good()
        {
            // Backup plan: in case user forgot to call Dispose()
            // Cleaning crew comes at night and frees the memory.
            if (_nativePtr != IntPtr.Zero)
            {
                Console.WriteLine("[Good Unmanaged] FINALIZER freeing unmanaged memory (user forgot Dispose).");
                Marshal.FreeHGlobal(_nativePtr);
                _nativePtr = IntPtr.Zero;
            }
        }
    }
}
