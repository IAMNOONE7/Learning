using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs_OOP.Basics
{
    /*
      Person is a simple example of a class with:
       - auto-properties (C# creates backing field)
       - computed property (FullName)
     */
    public class Person
    {
        // Auto-properties – simplest form
        public string FirstName { get; set; }
        public string LastName { get; set; }

        // This will be set later, so we allow set
        public int Age { get; set; }

        // Computed property – no set, only get
        // This does NOT store anything, it just returns combined string
        public string FullName
        {
            get { return $"{FirstName} {LastName}"; }
        }

        // Constructor – enforce that name is required
        public Person(string firstName, string lastName)
        {
            // here you could add validation if null/empty
            FirstName = firstName;
            LastName = lastName;
        }
    }
}
