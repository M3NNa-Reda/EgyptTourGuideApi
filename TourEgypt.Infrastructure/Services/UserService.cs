using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TourEgypt.Core.DTOs.User;
using TourEgypt.Core.Entities;
using TourEgypt.Core.Interfaces.Repositories;
using TourEgypt.Core.Interfaces.Services;

namespace TourEgypt.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;

        public UserService(
            ICurrentUserService currentUserService,
            IMapper mapper,
            UserManager<ApplicationUser> userManager,
            IUnitOfWork unitOfWork)
        {
            _currentUserService = currentUserService;
            _mapper = mapper;
            _userManager = userManager;
            _unitOfWork = unitOfWork;
        }
        private async Task<ApplicationUser> GetCurrentUserAsync()
        {
            var userId = GetCurrentUserId();

            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user == null)
                throw new KeyNotFoundException("User not found.");

            return user;
        }
        public async Task<UserProfileDto> GetProfileAsync()
        {
            var user = await GetCurrentUserAsync();

            var profile = _mapper.Map<UserProfileDto>(user);
            profile.SavedPlacesCount = await _unitOfWork.Favourites.CountByUserIdAsync(user.Id);
            profile.ReviewsCount = await _unitOfWork.Reviews.CountByUserIdAsync(user.Id);
            return profile;
        }

        private int GetCurrentUserId()
        {
            var userId = _currentUserService.UserId;

            if (!userId.HasValue)
            {
                throw new UnauthorizedAccessException("User is not authenticated or Token is invalid.");
            }

            return userId.Value;
        }


        public async Task UpdateProfileAsync(UpdateProfileDto dto)
        {
            var user = await GetCurrentUserAsync();

            if (!string.IsNullOrWhiteSpace(dto.FullName))
            {
                var names = dto.FullName.Trim().Split(' ', 2);

                user.FirstName = names[0];
                user.LastName = names.Length > 1 ? names[1] : string.Empty;
            }

            user.PhoneNumber = dto.PhoneNumber ?? user.PhoneNumber;
            user.Country = dto.Country ?? user.Country;
            user.City = dto.City ?? user.City;
            user.DateOfBirth = dto.DateOfBirth ?? user.DateOfBirth;
            user.Gender = dto.Gender ?? user.Gender;
            user.Bio = dto.Bio ?? user.Bio;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));

                throw new InvalidOperationException(errors);
            }
        }

        public async Task UpdateProfileImageAsync(IFormFile image)
        {
            var user = await GetCurrentUserAsync();

            if (image == null || image.Length == 0)
                throw new ArgumentException("Image is required.");

            var extension = Path.GetExtension(image.FileName).ToLower();

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };

            if (!allowedExtensions.Contains(extension))
                throw new ArgumentException("Only JPG and PNG images are allowed.");

            if (image.Length > 2 * 1024 * 1024)
                throw new ArgumentException("Maximum image size is 2 MB.");

            if (!string.IsNullOrEmpty(user.ProfileImageUrl))
            {
                var oldImagePath = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    user.ProfileImageUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));

                if (File.Exists(oldImagePath))
                    File.Delete(oldImagePath);
            }

            var folderPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                "images",
                "profiles");

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            var fileName = $"user_{user.Id}_{Guid.NewGuid()}{extension}";

            var filePath = Path.Combine(folderPath, fileName);

            using var stream = new FileStream(filePath, FileMode.Create);

            await image.CopyToAsync(stream);

            user.ProfileImageUrl = $"/images/profiles/{fileName}";

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));

                throw new InvalidOperationException(errors);
            }
        }
        public async Task DeleteProfileImageAsync()
        {
            var user = await GetCurrentUserAsync();

            if (!string.IsNullOrEmpty(user.ProfileImageUrl))
            {
                var imagePath = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    user.ProfileImageUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));

                if (File.Exists(imagePath))
                    File.Delete(imagePath);
            }

            user.ProfileImageUrl = null;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));

                throw new InvalidOperationException(errors);
            }
        }
        

    }
}
