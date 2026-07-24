using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using TourEgypt.Core.Entities;

namespace TourEgypt.Data.Configurations
{
    public class FavoriteConfiguration : IEntityTypeConfiguration<Favorite>
    {
        public void Configure(EntityTypeBuilder<Favorite> builder)
        {
            builder.HasKey(f => new { f.UserId, f.PlaceId });

            builder.HasOne(f => f.User)
                   .WithMany(u => u.Favorites)
                   .HasForeignKey(f => f.UserId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(f => f.Place)
                   .WithMany(p => p.Favorites)
                   .HasForeignKey(f => f.PlaceId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
