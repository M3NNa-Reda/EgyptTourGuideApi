using System;
using System.Collections.Generic;
using System.Text;
using TourEgypt.Core.Enums;

namespace TourEgypt.Core.DTOs.User
{
    public class UserProfileDto
    {
        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string? PhoneNumber { get; set; }

        public string? ProfileImageUrl { get; set; }

        public string? Country { get; set; }

        public string? City { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public Gender? Gender { get; set; }

        public string? Bio { get; set; }

        public string PreferredLanguage { get; set; } = "en";

        public int SavedPlacesCount { get; set; }

        public int ReviewsCount { get; set; }
    }
}
