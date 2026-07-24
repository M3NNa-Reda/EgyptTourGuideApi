using System;
using System.Collections.Generic;
using System.Text;
using TourEgypt.Core.DTOs.Auth;
using TourEgypt.Core.Entities;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TourEgypt.Core.Interfaces.Services
{
    public interface ITokenService
    {
        Task<AuthResponseDto> GenerateTokenAsync(
            ApplicationUser user,
            IList<string> roles);
    }
}
