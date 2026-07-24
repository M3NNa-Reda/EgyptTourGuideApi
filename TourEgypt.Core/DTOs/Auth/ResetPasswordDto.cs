using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TourEgypt.Core.DTOs.Auth
{
    public class ResetPasswordDto
    {
        public string Email { get; set; } = string.Empty;

        public int Code { get; set; }

        [Required]
        public string Password { get; set; } = string.Empty;

        [Compare(nameof(Password))]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
