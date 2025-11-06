using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs_OOP.Immutables
{
    /*
        Immutable class:
         - set values only in constructor
         - only getters
         - object is guaranteed to stay in valid state
       */
    public class ImmutableUser
    {
        public string Username { get; }
        public string Email { get; }
        public int Level { get; }

        public ImmutableUser(string username, string email, int level)
        {
            Username = username;
            Email = email;
            Level = level;
        }

        // no setters
        // if you need "to change", you create a NEW instance with modified value
        public ImmutableUser Promote()
        {
            return new ImmutableUser(Username, Email, Level + 1);
        }
    }
}
