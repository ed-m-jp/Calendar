using Calendar.DataAccess.EntityConfiguration;
using Calendar.shared.Entities;
using Calendar.Shared.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Calendar.DataAccess
{
    public class AppDbContext : IdentityDbContext<User>
    {
        public DbSet<Event> Events { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Set up the Entities that need extra configuration
            modelBuilder.ApplyConfiguration(new EventConfig());
        }
    }
}