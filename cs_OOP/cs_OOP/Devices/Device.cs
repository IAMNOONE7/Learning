using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs_OOP.Devices
{
    /*
      Abstract base class:
      - You CANNOT create "new Device()"
      - It defines COMMON properties and behavior
      - It can define abstract members that MUST be implemented
     */
    public abstract class Device
    {
        // common for all devices
        public string Name { get; protected set; }
        public string Manufacturer { get; protected set; }

        // ctor for common properties
        protected Device(string name, string manufacturer)
        {
            Name = name;
            Manufacturer = manufacturer;
        }

        // abstract = derived class MUST implement
        public abstract void Start();

        // virtual = base provides DEFAULT behavior, derived CAN override
        public virtual void Stop()
        {
            // default behavior
            System.Console.WriteLine($"{Name} stopping (default behavior).");
        }

        // non-virtual = derived classes inherit as-is
        public void PrintInfo()
        {
            System.Console.WriteLine($"Device: {Name}, made by {Manufacturer}");
        }
    }
}
