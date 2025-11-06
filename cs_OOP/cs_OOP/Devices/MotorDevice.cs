using cs_OOP.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs_OOP.Devices
{
    /*
    MotorDevice is a specific Device.
    - MUST implement Start() because it's abstract in base
    - Overrides Stop() to provide specialized behavior
   */
    public class MotorDevice : Device, IConnectable, IServiceable
    {
        private bool _isConnected;
        // IServiceable
        public int ServiceIntervalHours => 1000;
        public int RatedPowerKw { get; private set; }

        public MotorDevice(string name, string manufacturer, int ratedPowerKw)
            : base(name, manufacturer)   // call base ctor
        {
            RatedPowerKw = ratedPowerKw;
        }
        // required abstract implementation
        public override void Start()
        {
            if (!_isConnected)
            {
                Console.WriteLine($"{Name} cannot start – not connected.");
                return;
            }

            Console.WriteLine($"{Name} (Motor) is starting... Power: {RatedPowerKw} kW");
        }

        // override default Stop()
        public override void Stop()
        {
            Console.WriteLine($"{Name} (Motor) is decelerating...");
        }

        // IConnectable
        public void Connect()
        {
            _isConnected = true;
            Console.WriteLine($"{Name} connected.");
        }

        public void Disconnect()
        {
            _isConnected = false;
            Console.WriteLine($"{Name} disconnected.");
        }

        public bool IsConnected => _isConnected;

        // IServiceable
        public void PerformService()
        {
            Console.WriteLine($"{Name} service performed. Next in {ServiceIntervalHours} h.");
        }

    }
}
