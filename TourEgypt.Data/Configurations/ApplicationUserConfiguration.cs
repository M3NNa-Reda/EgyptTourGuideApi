using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using TourEgypt.Core.Entities;

namespace TourEgypt.Data.Configurations
{
    public class ApplicationUserConfiguration
    : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.Property(u => u.FirstName)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(u => u.LastName)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(u => u.ProfileImageUrl)
                   .HasMaxLength(500);

            builder.Property(u => u.Country)
                   .HasMaxLength(100);

            builder.Property(u => u.City)
                   .HasMaxLength(100);

            builder.Property(u => u.Bio)
                   .HasMaxLength(500);
        }
    }

}
