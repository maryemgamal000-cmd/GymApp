using GymSystem.DAL.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSystem.DAL.Data.Configurations
{
    internal class BookingConfiguration : IEntityTypeConfiguration<Booking>
    {
        public void Configure(EntityTypeBuilder<Booking> builder)
        {
            builder.Ignore(x => x.Id);
            builder.Property(c => c.CreatedAt).HasColumnName("BookingDate").HasDefaultValueSql("GETDATE()");

            builder.HasKey(x=>new {x.MemberId , x.SessionId});

        }
    }
}
