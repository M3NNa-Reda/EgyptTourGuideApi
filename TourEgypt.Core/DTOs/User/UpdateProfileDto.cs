using System;
using System.Collections.Generic;
using System.Text;
using TourEgypt.Core.Enums;

namespace TourEgypt.Core.DTOs.User
{
    public class UpdateProfileDto
    {
        public string? FullName { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }

        public string? Country { get; set; }

        public string? City { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public Gender? Gender { get; set; }

        public string? Bio { get; set; }
    }
}
