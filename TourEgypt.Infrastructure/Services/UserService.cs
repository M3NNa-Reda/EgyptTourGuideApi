using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using TourEgypt.Core.DTOs.User;
using TourEgypt.Core.Entities;
using TourEgypt.Core.Interfaces.Repositories;
using TourEgypt.Core.Interfaces.Services;

namespace TourEgypt.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;

        public UserService(IHttpContextAccessor httpContextAccessor,
            IMapper mapper,
            UserManager<ApplicationUser> userManager,IUnitOfWork unitOfWork)
        {
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
            _userManager = userManager;
            _unitOfWork = unitOfWork;
        }

        
        private int GetCurrentUserId()
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                throw new UnauthorizedAccessException("User is not authenticated or Token is invalid.");
            }

            return userId;
        }
        public async Task<UserProfileDto> GetProfileAsync()
        {
            var userId = GetCurrentUserId();
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user == null)
            {
                throw new KeyNotFoundException("User not found.");
            }

            
            return _mapper.Map<UserProfileDto>(user);
        }

        public async Task UpdateProfileAsync(UpdateProfileDto dto)
        {
            var userId = GetCurrentUserId();
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user == null)
                throw new KeyNotFoundException("User not found.");

            if (!string.IsNullOrWhiteSpace(dto.FullName))
            {
                var names = dto.FullName.Trim().Split(" ", 2);
                user.FirstName = names[0];
                user.LastName = names.Length > 1 ? names[1] : string.Empty;
            }

            user.PhoneNumber = dto.PhoneNumber ?? user.PhoneNumber;
            user.Country = dto.Country ?? user.Country;
            user.City = dto.City ?? user.City;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Failed to update profile: {errors}");
            }
        }
        

        public async Task UpdateProfileImageAsync(IFormFile image)
        {
            var userId = GetCurrentUserId();
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user == null)
                throw new KeyNotFoundException("User not found.");

            if (image == null || image.Length == 0)
                throw new ArgumentException("Invalid image file.");

            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "profiles");
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            var extension = Path.GetExtension(image.FileName);
            var fileName = $"user_{userId}_{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(folderPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }

            user.ProfileImageUrl = $"/images/profiles/{fileName}";
            await _userManager.UpdateAsync(user);
        }

        public async Task DeleteProfileImageAsync()
        {
            var userId = GetCurrentUserId();
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user == null)
                throw new KeyNotFoundException("User not found.");

            user.ProfileImageUrl = null;
            await _userManager.UpdateAsync(user);
        }
        public async Task SaveUserInterestsAsync(List<int> interestIds)
        {
            var userId = GetCurrentUserId();

            if (interestIds == null || !interestIds.Any())
                return;

            foreach (var categoryId in interestIds)
            {
                await _unitOfWork.UserInterests.AddAsync(new UserCategory
                {
                    UserId = userId,
                    CategoryId = categoryId
                });
            }

            await _unitOfWork.CompleteAsync();
        }



    }
}
