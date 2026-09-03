using GymSystem.DAL.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace GymSystem.DAL.Data.DBContexts
{
    public class GymDbContext : IdentityDbContext<ApplicationUser>
    {
        

        public GymDbContext(DbContextOptions options) : base(options)
        {
        }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseSqlServer("Server=.;Database=GymSystem;Trusted_connection=True;TrustServerCertificate=True");
        //}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            modelBuilder.Entity<ApplicationUser>(eb =>
            {
                eb.Property(x => x.FirstName)
                .HasColumnType("varchar")
                .HasMaxLength(50);

                eb.Property(x => x.LastName)
                 .HasColumnType("varchar")
                 .HasMaxLength(50);
            }


            );
        }


        public DbSet<Plan> Plans { get; set; } 
        public DbSet<Trainer> Trainers {  get; set; }
        public DbSet<Session> Sessions {  get; set; }   

        public DbSet<Membership> Memberships { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<HealthRecord> HealthRecords { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Booking> Bookings { get; set; }

       






    }
}
