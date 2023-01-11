using Microsoft.EntityFrameworkCore;
using University.Server.Domain.Models;

namespace University.Server.Domain.Persistence.Contexts
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Module> Modules { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<User>().ToTable("Users");
            builder.Entity<User>().HasKey(p => p.Id);
            builder.Entity<User>().Property(p => p.Id).IsRequired().ValueGeneratedOnAdd();

            builder.Entity<User>().HasData
            (
                new User { 
                    Id = Guid.NewGuid(), 
                    FirstName = "Max", 
                    LastName = "Mustermann",
                    Authorization = EAuthorization.Administrator
                }, // Id set manually due to in-memory provider
                new User
                {
                    Id = Guid.NewGuid(),
                    FirstName = "Bob",
                    LastName = "Mustermann",
                    Authorization = EAuthorization.Student
                }
            );

            builder.Entity<User>().ToTable("Modules");
            builder.Entity<User>().HasKey(p => p.Id);
            builder.Entity<User>().Property(p => p.Id).IsRequired().ValueGeneratedOnAdd();
        }
    }
}
