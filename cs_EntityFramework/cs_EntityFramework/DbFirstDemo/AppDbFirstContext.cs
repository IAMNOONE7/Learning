using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace cs_EntityFramework.DbFirstDemo;

public partial class AppDbFirstContext : DbContext
{
    public AppDbFirstContext()
    {
    }

    public AppDbFirstContext(DbContextOptions<AppDbFirstContext> options)
        : base(options)
    {
    }

    public virtual DbSet<EfmigrationsLock> EfmigrationsLocks { get; set; }

    public virtual DbSet<Post> Posts { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)

        => optionsBuilder.UseSqlite("");//db path

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<EfmigrationsLock>(entity =>
        {
            entity.ToTable("__EFMigrationsLock");

            entity.Property(e => e.Id).ValueGeneratedNever();
        });

        modelBuilder.Entity<Post>(entity =>
        {
            entity.HasIndex(e => e.UserId, "IX_Posts_UserId");

            entity.HasOne(d => d.User).WithMany(p => p.Posts).HasForeignKey(d => d.UserId);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
