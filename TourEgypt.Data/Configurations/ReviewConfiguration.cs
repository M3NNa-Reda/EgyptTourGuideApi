using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using TourEgypt.Core.Entities;

namespace TourEgypt.Data.Configurations
{
    public class ReviewConfiguration : IEntityTypeConfiguration<Review>
    {
        public void Configure(EntityTypeBuilder<Review> builder)
        {
            builder.HasKey(r => r.ReviewId);

            builder.Property(r => r.Content).HasMaxLength(500);
            builder.Property(r => r.Rating).IsRequired().HasPrecision(2, 1);
            builder.Property(r => r.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

            builder.HasOne(r => r.User)
                   .WithMany(u => u.Reviews)
                   .HasForeignKey(r => r.UserId) 
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(r => r.Place)
                   .WithMany(p => p.Reviews)
                   .HasForeignKey(r => r.PlaceId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
