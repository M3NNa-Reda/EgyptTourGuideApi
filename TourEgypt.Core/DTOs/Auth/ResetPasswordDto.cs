using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TourEgypt.Core.DTOs.Auth
{
    public class ResetPasswordDto
    {


        public string Email { get; set; } = string.Empty;

        public string NewPassword { get; set; } = string.Empty;
    }
}
