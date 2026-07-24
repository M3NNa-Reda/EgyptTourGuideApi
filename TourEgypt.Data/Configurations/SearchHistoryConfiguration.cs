using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System;
using System.Collections.Generic;
using System.Text;
using TourEgypt.Core.Entities;

namespace TourEgypt.Data.Configurations
{
    public class SearchHistoryConfiguration : IEntityTypeConfiguration<SearchHistory>
    {
        public void Configure(EntityTypeBuilder<SearchHistory> builder)
        {
            builder.Property(s => s.SearchText).IsRequired().HasMaxLength(200);
            builder.HasIndex(s => new { s.UserId, s.SearchDate });

            builder.HasOne(s => s.User)
                .WithMany(s => s.SearchHistories)
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(s => s.Place)
                .WithMany()
                .HasForeignKey(s => s.PlaceId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
