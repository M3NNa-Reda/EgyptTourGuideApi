using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using TourEgypt.Core.DTOs.User;

namespace TourEgypt.Core.DTOs.Auth
{
    public class AuthResponseDto
    {
        public string Token { get; set; } = string.Empty;


        public DateTime Expiration { get; set; }

        public UserDto User { get; set; } = default!;
    }
}
