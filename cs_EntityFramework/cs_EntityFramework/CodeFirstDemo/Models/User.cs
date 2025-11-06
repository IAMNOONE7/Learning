using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs_EntityFramework.CodeFirstDemo.Models
{
    // ===============================================
    // This class represents the structure of one "User"
    // ===============================================
    // Code-First EF rule: each public class with public
    // properties becomes a database table.
    // Here, table name will be "Users" by convention.
    public class User
    {
        // Primary key.
        // EF detects "Id" or "ClassNameId" as the PK automatically.
        public int Id { get; set; }
        // Normal columns: EF will create TEXT columns for strings.
        public string Name { get; set; } = "";
        public string Email { get; set; } = "";

        // Navigation property:
        // 1 User -> many Posts (one-to-many relationship).
        // EF will automatically link it to Post.UserId.
        public List<Post> Posts { get; set; } = new();
    }
}
