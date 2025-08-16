//using Domain.Entities;
//using Microsoft.EntityFrameworkCore;

//namespace Infrastructure.Persistence;

//public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
//{
//    public DbSet<User> Users => Set<User>();
//    public DbSet<ContaCorrente> ContaCorrentes => Set<ContaCorrente>();
//}

using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<ContaCorrente> ContaCorrente { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<User>().HasKey(c => c.Id);
            modelBuilder.Entity<ContaCorrente>().HasKey(c => c.IdContaCorrente);
        }
    }
}