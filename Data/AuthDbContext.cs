using AuthTest.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthTest.Data
{
    public class AuthDbContext : DbContext
    {
        private readonly IConfiguration _configuration;

        public AuthDbContext(IConfiguration configuration) 
        {
            _configuration = configuration; 
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }

        

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = _configuration.GetConnectionString("AuthDb");
            optionsBuilder.UseSqlite(connectionString);
        }
    }
}
