using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs_SOLID
{
    // Abstraction – OrderProcessor doesn't care if it's email, SMS, Teams, Slack...
    public interface IMessageService
    {
        void Send(string to, string message);
    }

    public class EmailMessageService : IMessageService
    {
        public void Send(string to, string message)
        {
            // Real life: this is like giving a letter to the post office.
            // Our OrderProcessor just says "send", it doesn't stand in the post line.
            Console.WriteLine($"[EMAIL to {to}] {message}");
        }
    }

    public class SmsMessageService : IMessageService
    {
        public void Send(string to, string message)
        {
            Console.WriteLine($"[SMS to {to}] {message}");
        }
    }
}
