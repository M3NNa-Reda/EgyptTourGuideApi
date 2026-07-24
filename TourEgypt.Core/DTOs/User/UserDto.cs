using System;
using System.Collections.Generic;
using System.Text;

namespace TourEgypt.Core.DTOs.User
{
    public class UserDto
    {
        public int Id { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string? PhoneNumber { get; set; }

        public string? ProfileImageUrl { get; set; }
        public string? Country { get; set; }
        public string? City { get; set; }
    }
}
