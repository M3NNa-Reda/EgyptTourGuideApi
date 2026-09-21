using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using TourEgypt.Core.Entities;

namespace TourEgypt.Data.Configurations
{
    public class PlaceConfiguration : IEntityTypeConfiguration<Place>
    {
        public void Configure(EntityTypeBuilder<Place> builder)
        {
            builder.HasKey(p => p.PlaceId);

            builder.Property(p => p.Name).IsRequired().HasMaxLength(150);
            builder.Property(p => p.Description).HasMaxLength(1000);
            builder.Property(p => p.Address).HasMaxLength(250);
            builder.Property(p => p.ImageUrl).HasMaxLength(500);
            builder.Property(p => p.Latitude).HasPrecision(18, 8);
            builder.Property(p => p.Longitude).HasPrecision(18, 8);
            builder.HasIndex(p => p.Name);


            builder.HasOne(p => p.City)
                   .WithMany(c => c.Places)
                   .HasForeignKey(p => p.CityId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.Category)
                   .WithMany(c => c.Places)
                   .HasForeignKey(p => p.CategoryId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
