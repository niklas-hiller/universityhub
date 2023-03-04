using Microsoft.EntityFrameworkCore;
using University.Server.Domain.Models;

namespace University.Server.Domain.Persistence.Contexts
{
    public class AppDbContext : DbContext
    {
        private readonly string _connectionString;

        public AppDbContext(DbContextOptions<AppDbContext> options, IConfiguration configuration)
            : base(options)
        {
            _connectionString = configuration["ConnectionString"] ?? throw new ArgumentNullException("ConnectionString");
        }

        public DbSet<Module> Modules { get; set; }
        public DbSet<ModuleAssignment> ModuleAssignments { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Lecture> Lectures { get; set; }
        public DbSet<SemesterModule> SemesterModules { get; set; }
        public DbSet<Semester> Semesters { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Module>()
                .ToContainer("Modules")
                .HasPartitionKey("Id")
                .HasKey(e => e.Id);
            builder.Entity<ModuleAssignment>()
                .ToContainer("ModuleAssignments")
                .HasPartitionKey("Id")
                .HasKey(e => e.Id);
            builder.Entity<User>()
                .ToContainer("Users")
                .HasPartitionKey("Id")
                .HasKey(e => e.Id);
            builder.Entity<Course>()
                .ToContainer("Courses")
                .HasPartitionKey("Id")
                .HasKey(e => e.Id);
            builder.Entity<Location>()
                .ToContainer("Locations")
                .HasPartitionKey("Id")
                .HasKey(e => e.Id);
            builder.Entity<Lecture>()
                .ToContainer("Lectures")
                .HasPartitionKey("Id")
                .HasKey(e => e.Id);
            builder.Entity<SemesterModule>()
                .ToContainer("SemesterModules")
                .HasPartitionKey("Id")
                .HasKey(e => e.Id);
            builder.Entity<Semester>()
                .ToContainer("Semesters")
                .HasPartitionKey("Id")
                .HasKey(e => e.Id);
        }

        protected override async void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string connectionString = _connectionString;
            string databaseName = "UniversityHub";

            optionsBuilder.UseCosmos(
                connectionString,
                databaseName,
                options =>
                {
                    // Configure additional Cosmos DB settings here, if needed.
                });


        }
    }
}
