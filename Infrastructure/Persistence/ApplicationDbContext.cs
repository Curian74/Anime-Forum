using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Persistence
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IConfigurationManager configurationManager) : IdentityDbContext<User, IdentityRole<Guid>, Guid>(options)
    {
        private readonly IConfigurationManager _configurationManager = configurationManager;

        public DbSet<Post> Posts { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var connectionString = _configurationManager.GetConnectionString("Default");
                optionsBuilder.UseSqlServer(connectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
