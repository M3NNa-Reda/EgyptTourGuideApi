using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using TourEgypt.Core.DTOs.User;

namespace TourEgypt.Core.Interfaces.Services
{
    public interface IUserService
    {
        Task<UserProfileDto> GetProfileAsync();

        Task UpdateProfileAsync(UpdateProfileDto dto);

        Task UpdateProfileImageAsync(IFormFile image);

        Task DeleteProfileImageAsync();
    }
}
