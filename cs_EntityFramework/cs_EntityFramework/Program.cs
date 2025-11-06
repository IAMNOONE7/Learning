using cs_EntityFramework.CodeFirstDemo;
using cs_EntityFramework.DbFirstDemo;
using Microsoft.EntityFrameworkCore;

namespace cs_EntityFramework
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // ===============================================
            // Every time we use EF, we create a new context.
            // "using" ensures it will be disposed automatically.
            // ===============================================
            using (var db = new AppDbContext())
            {
                // ------------------------------------------------------------------
                // CREATE new user if database is empty
                // (to make sure we have something to show)
                // ------------------------------------------------------------------
                var user = new CodeFirstDemo.Models.User
                {
                    Name = "John Doe",
                    Email = "john@example.com"
                };

                db.Users.Add(user);// marks it as "to be inserted"
                db.SaveChanges();// executes INSERT INTO Users...

                // CREATE post for this user
                var post = new CodeFirstDemo.Models.Post
                {
                    Title = "Hello EF Core",
                    Content = "This is my first post.",
                    UserId = user.Id
                };

                db.Posts.Add(post);
                db.SaveChanges();

                // ------------------------------------------------------------------
                // READ: load users and their posts
                // ------------------------------------------------------------------
                // Include() tells EF to eagerly load related data.
                // Without Include(), EF would only load the Users table.
                var usersWithPosts = db.Users
                    .Include(u => u.Posts)
                    .ToList();

                // ------------------------------------------------------------------
                // Display all users and their posts
                // ------------------------------------------------------------------
                foreach (var u in usersWithPosts)
                {
                    Console.WriteLine($"User: {u.Name} ({u.Email})");
                    foreach (var p in u.Posts)
                    {
                        Console.WriteLine($"  Post: {p.Title}");
                    }
                }
            }


            /*
        ========================================================
        DATABASE-FIRST
        ========================================================
        Goal:
        We already had a database (SQLite file app.db) that was created earlier.
        Now we wanted EF Core to GENERATE C# classes and a DbContext FROM that DB.

        Command used (Package Manager Console):
        Scaffold-DbContext "<connection string>" Microsoft.EntityFrameworkCore.Sqlite -OutputDir DbFirstDemo -Context AppDbFirstContext

        What EF generated:
         - AppDbFirstContext.cs  -> DbContext that knows about existing tables
         - One C# class per table (User, Post, ...)
         - Fluent API in OnModelCreating() based on existing schema

        When to use this:
         - DB was created first (by another app, DBA, legacy system)
         - We don't want to redesign the DB
         - We just need C# models to work with it

        Important notes:
         - Generated classes are "partial" - we can extend them without touching generated code.
         - If the DB changes, we can re-run Scaffold-DbContext to refresh the models.
         - This is the opposite of Code-First. In Code-First we design in C#, then migrate to DB.
           In Database-First we design in DB, then generate C#.
        ========================================================
        */

            using (var db = new AppDbFirstContext())
            {
                var users = db.Users
                    .Include(u => u.Posts)
                    .ToList();

                Console.WriteLine("=== DB-FIRST DATA ===");
                foreach (var u in users)
                {
                    Console.WriteLine($"User: {u.Name}");
                    foreach (var p in u.Posts)
                        Console.WriteLine($"   Post: {p.Title}");
                }
            }
        }
    }
}
