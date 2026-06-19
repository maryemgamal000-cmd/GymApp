using GymApp.Models;
using Microsoft.EntityFrameworkCore;

namespace GymApp.Configurations
{
    public class PlanConfiguration : IEntityTypeConfiguration<Plan>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Plan> builder)
        {
            builder.Property(x => x.Name).HasColumnType("Varchar").HasMaxLength(50);
            builder.Property(x => x.Description).HasMaxLength(200);
            builder.Property(x => x.Price).HasPrecision(10, 2);
            builder.Property(x => x.CreatedAt).HasDefaultValueSql("Getdate()");
        }
    }
}
