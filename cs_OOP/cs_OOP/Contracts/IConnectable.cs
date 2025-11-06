using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs_OOP.Contracts
{
    /*
      Interface = contract
      - only signatures
      - NO fields
      - class can implement multiple interfaces
     */
    public interface IConnectable
    {
        void Connect();
        void Disconnect();
        bool IsConnected { get; }
    }
}
