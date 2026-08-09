using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TourEgypt.Core.Entities;

namespace TourEgypt.Data.Configurations
{
    public class TourConfiguration : IEntityTypeConfiguration<Tour>
    {
        public void Configure(EntityTypeBuilder<Tour> builder)
        {
            // Primary Key
            builder.HasKey(t => t.TourId);

            // Basic properties
            builder.Property(t => t.Name)
                   .IsRequired()
                   .HasMaxLength(150);

            builder.Property(t => t.Description)
                   .HasMaxLength(1000);

            builder.Property(t => t.ImageUrl)
                   .HasMaxLength(500);

            // Price
            builder.Property(t => t.Price)
                   .HasPrecision(10, 2);

            // Duration
            builder.Property(t => t.DurationInHours)
                   .IsRequired();

            // Tour Type
            builder.Property(t => t.TourType)
                   .IsRequired()
                   .HasMaxLength(50);

            // Place → Tours
            builder.HasOne(t => t.Place)
                   .WithMany(p => p.Tours)
                   .HasForeignKey(t => t.PlaceId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Useful index
            builder.HasIndex(t => t.PlaceId);
        }
    }
}
