using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using TourEgypt.Core.Enums;

namespace TourEgypt.Core.Entities
{
    public class ApplicationUser : IdentityUser<int>
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? ProfileImageUrl { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? Country { get; set; }
        public string? City { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Bio { get; set; }
        public Gender? Gender { get; set; }
        public string PreferredLanguage { get; set; } = "en";

        public ICollection<Review> Reviews { get; set; } = new List<Review>();
        public ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
        public ICollection<UserCategory> UserCategories { get; set; } = new List<UserCategory>();
        public ICollection<SearchHistory> SearchHistories { get; set; } = new List<SearchHistory>();

    }
}

