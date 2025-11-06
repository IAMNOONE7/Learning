using cs_EntityFramework.CodeFirstDemo.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs_EntityFramework.CodeFirstDemo
{
    
    public class AppDbContext : DbContext
    {
        // ===============================================
        // This class is the "bridge" between C# code
        // and the database.
        //
        // DbContext represents a session with the database.
        // Through it, EF knows which classes should become tables,
        // and it handles reading/writing objects to the DB.
        // ===============================================

        // Each DbSet<T> corresponds to one table.
        // EF will track changes in these sets and generate SQL.
        public DbSet<User> Users => Set<User>();
        public DbSet<Post> Posts => Set<Post>();

        // -----------------------------------------------
        // OnConfiguring defines how EF connects to the DB.
        // Here, we use SQLite and store a local file "app.db".
        // -----------------------------------------------
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Get the directory where the .csproj is (one level up from bin)
            var baseDir = AppContext.BaseDirectory;           // .../bin/Debug/net9.0/
            var dbPath = Path.Combine(baseDir, "app.db");     // runtime db

            optionsBuilder.UseSqlite($"Data Source={dbPath}");
        }

        // -----------------------------------------------
        // OnModelCreating can be used to manually configure
        // table names, relationships, constraints, seed data, etc.
        // We don’t need it here, conventions are enough.
        // -----------------------------------------------
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {           
            base.OnModelCreating(modelBuilder);
        }
    }
}
