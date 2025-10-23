using Microsoft.EntityFrameworkCore;
using inventory_service.Models;

namespace inventory_service.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Inventory> Inventories { get; set; } = null!;
        public DbSet<Store> Stores { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Ensure table names and constraints from attributes are applied.
            // Additional model configuration can go here if needed.
        }
    }
}
