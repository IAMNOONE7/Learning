using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs_EntityFramework.CodeFirstDemo.Models
{
    // ===============================================
    // This class represents the structure of one "Post"
    // ===============================================
    // Code-First will create a "Posts" table based on this class.
    public class Post
    {
        // Primary key
        public int Id { get; set; }
        // Normal text columns
        public string Title { get; set; } = "";
        public string Content { get; set; } = "";

        // Foreign key column:
        // EF recognizes this as a link to the User table.
        public int UserId { get; set; }

        // Navigation property (back-reference to User)
        // Used when we want to load the author info of a Post.
        public User? User { get; set; }
    }
}
