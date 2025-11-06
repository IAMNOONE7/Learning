using cs_OOP.AdvancedProperties;
using cs_OOP.Basics;
using cs_OOP.Devices;
using cs_OOP.Encapsulation;
using cs_OOP.Immutables;
using cs_OOP.Methods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs_OOP
{
    /*
       ============================================================
       OOP PLAYGROUND – CLASSES & PROPERTIES
       ------------------------------------------------------------
       What is a class?
         - A template/blueprint for creating objects (instances).
         - A class usually holds DATA (fields/properties) and BEHAVIOR (methods).
     
       What is a property?
         - A “smart field”. It looks like a field from the outside,
           but inside it can run code in get / set.
         - Syntax: 
             public string Name { get; set; }
           This is an AUTO-PROPERTY. C# creates a hidden backing field for you.
     
       get:
         - Runs when someone READS the property.
         - You can return a calculated value, not just stored value.
     
       set:
         - Runs when someone WRITES the property.
         - Here you can VALIDATE, normalize, or even REJECT the value.
         - If you don’t want others to change the value, make the setter private:
             public string Name { get; private set; }
     
       When NOT to use public set:
         - When the property should not change after creation
         - When changing it must follow rules (use a method instead, e.g. Deposit())
         - When you want IMMUTABILITY (object cannot change after created)
     
       OOP ideas in this project:
         1. Encapsulation – hide fields, expose safe methods.
         2. Validation in setters – don’t allow invalid state.
         3. Read-only / private set – control who can modify.
         4. Computed properties – no storage, just calculation in get.
         5. Expression-bodied members – cleaner, modern properties/methods.
         6. Factory/static methods – when you don’t want “new” everywhere.
         7. Separation of data vs behavior – classes should DO something.
     
       NOTE:
         This is a console demo, so we’re putting everything in one place.
         In real apps you’d separate by domain (Customer, Order, …).
       ============================================================
     */
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== OOP Playground ===");

            // 1) Basic auto-properties, ctor
            BasicsDemo();

            // 2) Encapsulation with private set + methods
            EncapsulationDemo();

            // 3) Advanced properties (validation, computed, read-only)
            AdvancedPropertiesDemo();

            // 4) Class methods (instance vs static)
            MethodsDemo();

            InheritanceDemo();
            ImmutabilityDemo();


            Console.WriteLine("\nDone. Press any key...");
            Console.ReadKey();
        }

        static void BasicsDemo()
        {
            Console.WriteLine("\n--- Basics Demo ---");
            var p = new Person("Jan", "Novák");
            p.Age = 30;  // using set           
            Console.WriteLine($"Full name: {p.FullName}");
            Console.WriteLine($"Age: {p.Age}");
        }

        static void EncapsulationDemo()
        {
            Console.WriteLine("\n--- Encapsulation Demo ---");
            var acc = new BankAccount("CZK", 1000m);
            acc.Deposit(500m);
            // acc.Balance = 0;  // - this should NOT be allowed from outside
            Console.WriteLine($"Balance after deposit: {acc.Balance}");

            bool ok = acc.Withdraw(2000m);
            Console.WriteLine($"Withdraw 2000 result: {ok}, balance: {acc.Balance}");
        }

        static void AdvancedPropertiesDemo()
        {
            Console.WriteLine("\n--- Advanced Properties Demo ---");
            var prod = new Product("P-100", "Industrial Sensor", 1200m);
            Console.WriteLine($"Product: {prod.DisplayName} | Price: {prod.Price} | With VAT: {prod.PriceWithVat}");

            // try invalid price
            prod.Price = -50;   // setter will correct / reject
            Console.WriteLine($"After invalid price: {prod.Price}");

            // try invalid price
            prod.Price = 1100;   // setter will correct / reject
            Console.WriteLine($"After invalid price: {prod.Price}");

            // try invalid price
            prod.Price = 1300;   // setter will correct / reject
            Console.WriteLine($"After invalid price: {prod.Price}");
        }

        static void MethodsDemo()
        {
            Console.WriteLine("\n--- Methods Demo ---");
            var calc = new Calculator();
            int sum = calc.Add(10, 20);
            Console.WriteLine($"Instance Add: {sum}");

            int staticSum = Calculator.AddStatic(5, 5);
            Console.WriteLine($"Static Add: {staticSum}");
        }

        static void InheritanceDemo()
        {
            Console.WriteLine("\n--- Inheritance / Abstract / Interfaces Demo ---");
            var motor = new MotorDevice("ConveyorMotor1", "Rockwell Automation", 5);

            motor.PrintInfo();      // from base
            motor.Connect();        // from IConnectable
            motor.Start();          // override
            motor.PerformService(); // from IServiceable
            motor.Stop();           // override
        }

        static void ImmutabilityDemo()
        {
            Console.WriteLine("\n--- Immutability Demo ---");
            var user = new ImmutableUser("jan.n", "jan@example.com", 1);
            Console.WriteLine($"User: {user.Username}, level: {user.Level}");

            var promoted = user.Promote();
            Console.WriteLine($"Promoted: {promoted.Username}, level: {promoted.Level}");
        }
    }
}
