# cs_MemoryManagement

A collection of small console demos for understanding how memory really works in .NET.  
Each folder is a focused, hands-on experiment — not theory.  
All examples are written with comments and real-life analogies.

---

## Projects Overview

### **01_ReferenceVsValue**
- Shows how **classes** (reference types) and **structs** (value types) behave differently.
- Copying a class - copies the *reference* (both point to same object).
- Copying a struct - copies the *data* (two independent copies).
- Real-life: two keys to one house (class) vs. two photocopies of a paper (struct).

---

### **02_GC_Basics**
- Demonstrates **Garbage Collection** in action.
- Uses `GC.GetTotalMemory`, `GC.Collect`, and object generations.
- Explains Gen0, Gen1, Gen2, and when GC runs.
- Real-life: cleaning crew that removes unused stuff when space gets full.

---

### **03_FinalizersAndDispose**
- Difference between **finalizers** and **IDisposable**.
- Shows the full **Dispose pattern** (`Dispose(bool disposing)` + `GC.SuppressFinalize`).
- Finalizer = “cleaning crew will do it someday”.
- Dispose = “I’ll clean my desk before I leave”.
- Real-life: office cleanup vs. waiting for the night crew.

---

### **04_ManagedVsUnmanaged**
- Explains **managed memory** (GC-controlled) vs **unmanaged resources** (files, handles, native memory).
- Simulates unmanaged memory with `Marshal.AllocHGlobal` and proper freeing.
- Shows why `Dispose` exists even with GC.
- Real-life: robot cleaner knows how to tidy your desk, but not how to unplug the coffee machine.

---

### **05_MemoryLeakInDotNet**
- Demonstrates **leaks** in managed code.
- GC can’t collect objects still referenced by a static list or event subscription.
- Adds 1000 objects - still “alive” after GC - freed only when list is cleared.
- Real-life: you keep old junk on your desk — cleaner sees it, so leaves it there forever.

---

### **06_LargeObjectsAndArrays**
- Shows what happens when you allocate large arrays (> ~85 KB).
- Introduces the **Large Object Heap (LOH)** and why it’s not compacted.
- Demonstrates memory “ratcheting up” as big buffers come and go.
- Real-life: small boxes easy to move; big couches go to a separate warehouse
