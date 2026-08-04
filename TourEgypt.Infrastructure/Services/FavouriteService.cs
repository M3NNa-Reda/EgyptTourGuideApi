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
            if (placeId <= 0)
                throw new ArgumentException("Invalid place id.");

            var userId = GetUserId();

            var place = await _unitOfWork.Places.GetByIdAsync(placeId);
            if (place == null)
                throw new KeyNotFoundException("Place not found.");

            var alreadyFavourite = await _unitOfWork.Favourites.IsFavouriteAsync(userId, placeId);
            if (alreadyFavourite)
                return;

            place.favoriteCount += 1;
            _unitOfWork.Places.Update(place);

            var favourite = new Favorite { UserId = userId, PlaceId = placeId };
            await _unitOfWork.Favourites.AddAsync(favourite);
            await _unitOfWork.CompleteAsync();
        }

        public async Task RemoveFavouriteAsync(int placeId)
        {
            if (placeId <= 0)
                throw new ArgumentException("Invalid place id.");

            var userId = GetUserId();

            var isFavourite = await _unitOfWork.Favourites.IsFavouriteAsync(userId, placeId);
            if (!isFavourite)
                throw new KeyNotFoundException("Favourite not found.");

            var place = await _unitOfWork.Places.GetByIdAsync(placeId);
            if (place == null)
                throw new KeyNotFoundException("Place not found.");

            if (place.favoriteCount > 0)
                place.favoriteCount -= 1;

            _unitOfWork.Places.Update(place);

            var favourite = new Favorite { UserId = userId, PlaceId = placeId };
             _unitOfWork.Favourites.Delete(favourite);
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
                Address = f.Place.Address,
                ImageUrl = f.Place.ImageUrl,
                AverageRating = f.Place.AverageRating,
                IsSaved = true,
                CuisineType = f.Place.CuisineType
            });
        }

        public async Task<bool> IsFavouriteAsync(int placeId)
        {
            var userId = GetUserId();
            return await _unitOfWork.Favourites.IsFavouriteAsync(userId, placeId);
        }
    }
}