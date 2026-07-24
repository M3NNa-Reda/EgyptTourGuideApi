using System;
using System.Collections.Generic;
using System.Text;

namespace TourEgypt.Core.DTOs.Auth
{
    public class VerifyCodeDto
    {
        public string Email { get; set; } = string.Empty;

        public int Code { get; set; }
    }
}
