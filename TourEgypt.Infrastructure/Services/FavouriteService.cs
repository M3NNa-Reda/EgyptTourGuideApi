using System;
using System.Collections.Generic;
using System.Linq;
using TourEgypt.Core.DTOs.Place;
using TourEgypt.Core.Entities;
using TourEgypt.Core.Interfaces.Repositories;
using TourEgypt.Core.Interfaces.Services;

namespace TourEgypt.Infrastructure.Services
{
    public class FavouriteService : IFavouriteService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public FavouriteService(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        private int GetUserId()
        {
            var userId = _currentUserService.UserId;
            if (!userId.HasValue)
                throw new UnauthorizedAccessException("User is not authenticated.");

            return userId.Value;
        }

        public async Task AddFavouriteAsync(int placeId)
        {
            if (placeId <= 0)
                throw new ArgumentException("Invalid place id.");

            var userId = GetUserId();

            var alreadyFavourite = await _unitOfWork.Favourites.IsFavouriteAsync(userId, placeId);
            if (alreadyFavourite)
                return;

            var place = await _unitOfWork.Places.GetByIdAsync(placeId);
            if (place == null)
                throw new KeyNotFoundException("Place not found.");

            place.FavoriteCount += 1;
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

            var favourites = await _unitOfWork.Favourites.GetAllByUserIdAsync(userId);
            var favourite = favourites.FirstOrDefault(f => f.PlaceId == placeId);

            if (favourite == null)
                throw new KeyNotFoundException("Favourite not found.");

            var place = await _unitOfWork.Places.GetByIdAsync(placeId);
            if (place != null && place.FavoriteCount > 0)
            {
                place.FavoriteCount -= 1;
                _unitOfWork.Places.Update(place);
            }

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