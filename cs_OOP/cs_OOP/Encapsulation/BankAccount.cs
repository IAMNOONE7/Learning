using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs_OOP.Encapsulation
{
    /*
     BankAccount demonstrates:
      - private set - only this class can change Balance
      - public method to change internal state (Deposit/Withdraw)
      - validation - we don't allow negative deposits
      - this is typical OOP: hide data, expose behavior
    */
    public class BankAccount
    {
        // Currency shouldn't change after creation - private set or read-only
        public string Currency { get; private set; }

        // Balance is readable, but NOT settable from outside
        public decimal Balance { get; private set; }

        public BankAccount(string currency, decimal initialBalance)
        {
            Currency = currency;
            Balance = initialBalance;
        }

        public void Deposit(decimal amount)
        {
            if (amount <= 0)
                return; // or throw exception

            Balance += amount;
        }

        public bool Withdraw(decimal amount)
        {
            if (amount <= 0)
                return false;

            if (amount > Balance)
                return false;

            Balance -= amount;
            return true;
        }
    }
}
