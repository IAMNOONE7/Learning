using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _01_ReferenceVsValue
{
    /*
    ==================================================================================
    REFERENCE vs VALUE TYPES
    ==================================================================================

    The difference is not about “where they live” (heap vs stack)
       — that’s an implementation detail.
       The real difference is how assignment and method calls behave.

      Value type = "copy the value"
      Reference type = "copy the reference (address)"

    ----------------------------------------------------------------------------------
    VALUE TYPES  (copied)
    ----------------------------------------------------------------------------------
     Examples (built-in):
       - int, double, bool, char, struct, enum, decimal, DateTime

     When I assign or pass them to a method → the whole value is copied.
       So changing the copy doesn’t affect the original.

     Analogy:
       - Imagine I wrote my friend’s phone number on a paper.
         If I make another paper with the same number and change it later,
         the original paper stays the same. Two independent copies.

    Typical use:
       - Small, simple data containers (coordinates, color, money, etc.)
       - Things that behave like numbers, not like objects.

    Memory:
       - Usually stored directly on the stack (if local),
         or inline inside other objects (like in an array or struct).

    ----------------------------------------------------------------------------------
    REFERENCE TYPES  (shared)
    ----------------------------------------------------------------------------------
     Examples:
       - class, interface, delegate, string, object, arrays, records (class)

     When I assign or pass them → only the reference (address) is copied.
       Both variables point to the same object in memory.
       So changes through one variable are visible through all.

    Analogy:
       - Two friends share one key to the same apartment.
         If one rearranges furniture, the other sees the new layout too.

    Typical use:
       - Complex objects that can change internally (User, Device, Car, etc.)
       - Collections (List<>, Dictionary<>, arrays)
       - Anything that can be null or needs shared identity.

    Memory:
       - The reference variable lives on the stack,
         but it points to data stored on the managed heap.
       - The heap object’s lifetime is managed by the Garbage Collector (GC).

    ----------------------------------------------------------------------------------
    EXTRAS AND GOTCHAS
    ----------------------------------------------------------------------------------

    1. string — is actually a reference type but immutable.
       So it behaves “like a value type” when you modify it,
       because each change creates a new string object.

    2. structs in collections — if you store a struct in a List<>,
       and then modify a copy returned by indexer, the original is not changed!
       Example:
         var list = new List<PointStruct> { new PointStruct { X = 1, Y = 2 } };
         var p = list[0];
         p.X = 99; // modifies the copy, not the list item
         // list[0].X is still 1

    3. Passing by ref / out / in keywords
       - Even value types can be passed "by reference" explicitly.
       - Example:
           void AddOne(ref int x) { x++; }
           AddOne(ref number);
       - That tells the compiler to give the method direct access to the caller’s variable.

    4. Boxing
       - When a value type is treated as an object (e.g., stored in an object variable),
         it gets “boxed” = wrapped in a heap object.
       - Example:
           object o = 123;  // int (value) -> boxed into object (reference)
           int x = (int)o;  // unboxed

       - Boxing = allocation, slower. Avoid in performance-critical paths.

    ----------------------------------------------------------------------------------
    Quick summary table

    | Type Category | Examples                         | Assigned / Passed As | Behavior After Assignment |
    |----------------|----------------------------------|----------------------|---------------------------|
    | Value types    | int, double, bool, struct, enum  | Copy of data         | Independent copy          |
    | Reference types| class, string, array, List<>     | Copy of address      | Shared same instance      |

    ----------------------------------------------------------------------------------
     Real-world analogy summary:

    | Concept             | Analogy                                                   |
    |----------------------|-----------------------------------------------------------|
    | Value type           | Two photocopies of a document — edit one, other unchanged |
    | Reference type       | Two keys to same house — both access same furniture       |
    | ref / out parameter  | Lending the *actual* object, not a copy                   |
    | string (immutable)   | Once printed, you can’t change it — you make a new print  |
    | Boxing               | Putting the value into a box labeled “object”             |

    ==================================================================================
    */


    // This console app is ONLY about understanding what is copied and what is referenced.
    // If I understand THIS, then later GC, IDisposable, leaking, etc. makes way more sense
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== 01 - Reference vs Value ===");
            Console.WriteLine();

            // 1) CLASS = reference type
            DemoClassBehavior();

            Console.WriteLine();
            Console.WriteLine("--------------------------------------");
            Console.WriteLine();

            // 2) STRUCT = value type
            DemoStructBehavior();

            Console.WriteLine();
            Console.WriteLine("--------------------------------------");
            Console.WriteLine();

            // 3) Passing to methods - do we change original or just a copy?
            DemoMethodParameterBehavior();

            Console.WriteLine();
            Console.WriteLine("Done. Press any key...");
            Console.ReadKey();
        }

        static void DemoClassBehavior()
        {
            Console.WriteLine(">>> DEMO: CLASS (reference type)");

            // Create first person (class = reference type)
            var p1 = new PersonClass { Name = "Alice", Age = 30 };

            // Now I make a "copy"
            // BUT because it's a CLASS, I'm not copying the whole person,
            // I'm just copying the ADDRESS (reference) to the person living on the heap.
            var p2 = p1;

            Console.WriteLine($"p1: {p1}");
            Console.WriteLine($"p2: {p2}");

            Console.WriteLine("-- Change p2.Name to 'Bob' --");
            p2.Name = "Bob";

            // Now BOTH p1 and p2 will show Name = Bob, because they're pointing to the SAME object.
            // Real life analogy:
            //   This is like having 2 phone contacts that point to the SAME person.
            //   If that person changes their haircut, both contacts "see" the new haircut.
            Console.WriteLine($"p1 after change: {p1}");
            Console.WriteLine($"p2 after change: {p2}");
        }

        static void DemoStructBehavior()
        {
            Console.WriteLine(">>> DEMO: STRUCT (value type)");

            // Create first point (struct = value type)
            var s1 = new PointStruct { X = 10, Y = 20 };

            // Now I "copy" it
            // Because it's a STRUCT, I get a FULL COPY of the data.
            // After this line: s1 and s2 are TWO SEPARATE boxes with numbers.
            var s2 = s1;

            Console.WriteLine($"s1: {s1}");
            Console.WriteLine($"s2: {s2}");

            Console.WriteLine("-- Change s2.X to 999 --");
            s2.X = 999;

            // Now only s2 is changed.
            // Real life analogy:
            //   This is like photocopying a paper. If I draw on the copy,
            //   the original paper doesn't change.
            Console.WriteLine($"s1 after change: {s1}");
            Console.WriteLine($"s2 after change: {s2}");
        }

        static void DemoMethodParameterBehavior()
        {
            Console.WriteLine(">>> DEMO: Passing to methods");

            var person = new PersonClass { Name = "Charlie", Age = 25 };
            var point = new PointStruct { X = 5, Y = 5 };

            Console.WriteLine($"person BEFORE method: {person}");
            Console.WriteLine($"point  BEFORE method: {point}");

            // This method will modify the person (class)
            ModifyPerson(person);

            // This method will modify the struct parameter (but only its local copy)
            ModifyPoint(point);

            Console.WriteLine("-- AFTER calling methods --");
            Console.WriteLine($"person AFTER method: {person}");
            Console.WriteLine($"point  AFTER method: {point}");

            // Notice:
            // - person changed, because we passed a reference -> method touched the same object
            // - point did NOT change, because method got a copy
            //
            // Real life analogy:
            //   - Class: I gave someone the ADDRESS of my house. They went there and painted the walls.
            //     When I come home, the walls are painted.
            //   - Struct: I gave someone a PHOTO of my house. They painted on the photo.
            //     My real house is unchanged.
        }

        static void ModifyPerson(PersonClass p)
        {
            // Even though the parameter is a "copy" of the reference,
            // it still points to the SAME object.
            // So changing properties here changes the original object.
            p.Age += 1;
            p.Name += " (updated)";
        }

        static void ModifyPoint(PointStruct p)
        {
            // Here I'm modifying ONLY the local copy.
            // Outside caller will NOT see this.
            p.X += 10;
            p.Y += 10;
        }
    }

    // =========================
    // REFERENCE TYPE EXAMPLE
    // =========================
    // In real apps, most of our models are classes.
    // They usually represent "things that can change" -> users, orders, devices, etc.
    // Classes live on the heap and variables hold references to them.
    class PersonClass
    {
        public string Name { get; set; }
        public int Age { get; set; }

        // Just for prettier printing
        public override string ToString()
        {
            return $"PersonClass(Name='{Name}', Age={Age})";
        }
    }

    // =========================
    // VALUE TYPE EXAMPLE
    // =========================
    // Structs are great for small, immutable-ish data like coordinates, colors, money amounts, etc.
    // But we must remember: assignment = copy.
    struct PointStruct
    {
        public int X;
        public int Y;

        public override string ToString()
        {
            return $"PointStruct(X={X}, Y={Y})";
        }
    }
}
