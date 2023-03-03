using Microsoft.EntityFrameworkCore;
using University.Server.Domain.Models;

namespace University.Server.Domain.Persistence.Contexts
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Module> Modules { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Semester> Semesters { get; set; }

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

            builder.Entity<Module>().ToTable("Modules");
            builder.Entity<Module>().HasKey(p => p.Id);
            builder.Entity<Module>().Property(p => p.Id).IsRequired().ValueGeneratedOnAdd();

            builder.Entity<Location>().ToTable("Locations");
            builder.Entity<Location>().HasKey(p => p.Id);
            builder.Entity<Location>().Property(p => p.Id).IsRequired().ValueGeneratedOnAdd();

            builder.Entity<Semester>().ToTable("Semesters");
            builder.Entity<Semester>().HasKey(p => p.Id);
            builder.Entity<Semester>().Property(p => p.Id).IsRequired().ValueGeneratedOnAdd();

            builder.Entity<Course>().ToTable("Courses");
            builder.Entity<Course>().HasKey(p => p.Id);
            builder.Entity<Course>().Property(p => p.Id).IsRequired().ValueGeneratedOnAdd();
        }
    }
}
