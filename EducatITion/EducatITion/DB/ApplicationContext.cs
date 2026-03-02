using EducatITion.DB.Models;
using Microsoft.EntityFrameworkCore;

namespace EducatITion.DB
{
    public class ApplicationContext : DbContext
	{
		public DbSet<User> Users { get; set; }
		public DbSet<Course> Courses { get; set; }

		public ApplicationContext(DbContextOptions<ApplicationContext> options)
			: base(options)
		{
			Database.EnsureCreated();
		}

		public ApplicationContext()
			: base()
		{
			Database.EnsureCreated();
		}

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                IConfigurationRoot configuration = new ConfigurationBuilder()
                   .SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("appsettings.json")
                   .Build();
                var connectionString = configuration.GetConnectionString("DefaultConnection");
                optionsBuilder.UseSqlServer(connectionString);
            }
        }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<User>()
				.Property(u => u.Id)
				.ValueGeneratedOnAdd();

			modelBuilder.Entity<Course>()
				.Property(u => u.Id)
				.ValueGeneratedOnAdd();
		}
	}
}
