using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using TourEgypt.Core.DTOs.Place;
using TourEgypt.Core.Entities;
using TourEgypt.Core.Interfaces.Repositories;
using TourEgypt.Core.Interfaces.Services;

namespace TourEgypt.Infrastructure.Services
{
    public class FavouriteService : IFavouriteService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public FavouriteService(
            IUnitOfWork unitOfWork,
            IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
        }

        private int GetUserId()
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User
                .FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
                throw new UnauthorizedAccessException("User ID not found in token.");

            return int.Parse(userIdClaim.Value);
        }

        public async Task AddFavouriteAsync(int placeId)
        {
            var userId = GetUserId();
            var favourite = new Favorite { UserId = userId, PlaceId = placeId };
            await _unitOfWork.Favourites.AddAsync(favourite);
            await _unitOfWork.CompleteAsync();
        }

        public async Task RemoveFavouriteAsync(int placeId)
        {
            var userId = GetUserId();
            var favourite = new Favorite { UserId = userId, PlaceId = placeId };
            await _unitOfWork.Favourites.RemoveAsync(favourite);
            await _unitOfWork.CompleteAsync();
        }

        public async Task<IEnumerable<PlaceCardDto>> GetAllFavouritesAsync()
        {
            var userId = GetUserId();
            var favourites = await _unitOfWork.Favourites.GetAllByUserIdAsync(userId);
            return favourites.Select(f => new PlaceCardDto
            {
                Id = f.Place.PlaceId,
                Name = f.Place.Name,
                ImageUrl = f.Place.ImageUrl,
                AverageRating = f.Place.AverageRating
            });
        }

        public async Task<bool> IsFavouriteAsync(int placeId)
        {
            var userId = GetUserId();
            return await _unitOfWork.Favourites.IsFavouriteAsync(userId, placeId);
        }
    }
}