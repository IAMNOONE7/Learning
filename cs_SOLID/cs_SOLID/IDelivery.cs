using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs_SOLID
{
    // Base abstraction for delivery
    // LSP rule: any class that implements IDelivery must be able to "Deliver" in a meaningful way.
    public interface IDelivery
    {
        void Deliver();
    }
    // Real life: delivering via courier company
    public class CourierDelivery : IDelivery
    {
        private readonly string _address;
        private readonly string _courierName;

        public CourierDelivery(string address, string courierName)
        {
            _address = address;
            _courierName = courierName;
        }

        public void Deliver()
        {
            Console.WriteLine($"Sending package to '{_address}' via '{_courierName}'...");
        }
    }

    // Real life: customer picks it up in store
    public class PickupDelivery : IDelivery
    {
        private readonly string _pickupPoint;

        public PickupDelivery(string pickupPoint)
        {
            _pickupPoint = pickupPoint;
        }

        public void Deliver()
        {
            Console.WriteLine($"Order will be ready for pickup at '{_pickupPoint}'.");
        }
    }

    // BAD example (just for explanation, don't use it):
    // If we made a delivery that throws NotSupportedException in Deliver()
    // it would violate LSP – because code expects that "delivery.Deliver()" always works.
}
