using GymApp.Configurations;
using GymApp.Models;
using Microsoft.EntityFrameworkCore;

namespace GymApp.DBContexts
{
    public class GymDbContext : DbContext
    {

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=.;Database=GymSystem;Trusted_connection=True;TrustServerCertificate=True");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration<Plan>(new PlanConfiguration());
        }


        public DbSet<Plan> Plans { get; set; } 
    }
}
