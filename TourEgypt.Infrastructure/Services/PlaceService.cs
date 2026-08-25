using AutoMapper;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using TourEgypt.Core.Common;
using TourEgypt.Core.DTOs.Place;
using TourEgypt.Core.Entities;
using TourEgypt.Core.Interfaces.Repositories;
using TourEgypt.Core.Interfaces.Services;

namespace TourEgypt.Infrastructure.Services
{
    public class PlaceService : IPlaceService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;

        public PlaceService(IMapper mapper, IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _currentUserService = currentUserService;
        }


        public async Task<IReadOnlyList<NearbyPlaceDto>> GetNearbyPlacesAsync(double latitude, double longitude, double maxDistanceInKm)
        {
            if (latitude < -90 || latitude > 90 ||
                longitude < -180 || longitude > 180)
                throw new ArgumentException("Invalid coordinates.");

            if (maxDistanceInKm <= 0)
                maxDistanceInKm = 10;

            if (maxDistanceInKm > 100)
                throw new ArgumentException("Maximum distance is 100 km.");

            var places = await _unitOfWork.Places
                .GetNearbyAsync(latitude, longitude, maxDistanceInKm);

            var result = places
                .Select(p =>
                {
                    var dto = _mapper.Map<NearbyPlaceDto>(p);

                    dto.DistanceInKm = Math.Round(
                        GeoHelper.CalculateDistance(
                            latitude,
                            longitude,
                            p.Latitude,
                            p.Longitude),
                        1);

                    return dto;
                })
                .Where(p => p.DistanceInKm <= maxDistanceInKm)
                .OrderBy(p => p.DistanceInKm)
                .ToList();

            await SetIsSavedStatusForNearbyAsync(result);

            return result;
        }

        public async Task<PlaceDetailsDto?> GetPlaceByIdAsync(int id)
        {
            var place = await _unitOfWork.Places.GetByIdAsync(id);

            if (place == null)
                throw new KeyNotFoundException("Place not found");

            var dto = _mapper.Map<PlaceDetailsDto>(place);
            
            var userId = _currentUserService.UserId;
            if (userId.HasValue)
            {
                dto.IsSaved = await _unitOfWork.Favourites.IsFavouriteAsync(userId.Value, id);
            }

            return dto;
        }

        public async Task<IReadOnlyList<PlaceCardDto>> GetPlacesByCategoryAsync(int categoryId, int page, int pageSize)
        {
            if (categoryId <= 0)
                throw new ArgumentException("Invalid category ID.");
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 10;
            var places = await _unitOfWork.Places.GetByCategoryAsync(categoryId, page, pageSize);
            var dtos = _mapper.Map<List<PlaceCardDto>>(places);

            await SetIsSavedStatusAsync(dtos);

            return dtos;
        }

        public async Task<IReadOnlyList<PlaceCardDto>> SearchPlacesAsync(string keyword, int count)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                throw new ArgumentException("Keyword is required.");
            if (count <= 0) count = 5;
            var places = await _unitOfWork.Places.SearchAsync(keyword, count);
            var dtos = _mapper.Map<List<PlaceCardDto>>(places);

            await SetIsSavedStatusAsync(dtos);

            return dtos;
        }
        public async Task<int> CreatePlaceAsync(SavePlaceDto placeDto)
        {
            var placeEntity = _mapper.Map<Place>(placeDto);
            await _unitOfWork.Places.AddAsync(placeEntity);
            await _unitOfWork.CompleteAsync();
            return placeEntity.PlaceId;

        }


        public async Task UpdatePlaceAsync(int id, SavePlaceDto placeDto)
        {
            var place = await _unitOfWork.Places.GetByIdAsync(id);

            if (place == null)
                throw new KeyNotFoundException("Place not found");

            _mapper.Map(placeDto, place);

            _unitOfWork.Places.Update(place);

            await _unitOfWork.CompleteAsync();
        }
        public async Task DeletePlaceAsync(int id)
        {
            var place = await _unitOfWork.Places.GetByIdAsync(id);

            if (place == null)
                throw new KeyNotFoundException("Place not found");

            
            _unitOfWork.Places.Delete(place);

            await _unitOfWork.CompleteAsync();
        }
        private async Task SetIsSavedStatusAsync(List<PlaceCardDto> dtos)
        {
            var userId = _currentUserService.UserId;
            if (userId.HasValue && dtos.Any())
            {
                var userFavoritePlaceIds = await _unitOfWork.Favourites.GetUserFavoritePlaceIdsAsync(userId.Value);
                var favoriteSet = new HashSet<int>(userFavoritePlaceIds);

                foreach (var dto in dtos)
                {
                    dto.IsSaved = favoriteSet.Contains(dto.Id);
                }
            }
        }
        private async Task SetIsSavedStatusForNearbyAsync(List<NearbyPlaceDto> dtos)
        {
            var userId = _currentUserService.UserId;
            if (userId.HasValue && dtos.Any())
            {
                var userFavoritePlaceIds = await _unitOfWork.Favourites.GetUserFavoritePlaceIdsAsync(userId.Value);
                var favoriteSet = new HashSet<int>(userFavoritePlaceIds);

                foreach (var dto in dtos)
                {
                    dto.IsSaved = favoriteSet.Contains(dto.Id);
                }
            }
        }

        public async Task UpdatePlaceMetricsAsync()
        {
            var places = await _unitOfWork.Places.GetAllPlacesWithReviewsAsync();

            foreach (var place in places)
            {
                if (place.Reviews == null || !place.Reviews.Any())
                {
                    place.ReviewsCount = 0;
                    place.AverageRating = 0;
                    continue;
                }

                place.ReviewsCount = place.Reviews.Count;
                place.AverageRating = Math.Round(place.Reviews.Average(r => r.Rating), 1);
            }

            await _unitOfWork.CompleteAsync();
        }

        public async Task<IReadOnlyList<PlaceCardDto>> GetTopPlacesAsync(int page, int pageSize)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 10;

            var places = await _unitOfWork.Places.GetTopPlacesAsync(page, pageSize);
            var dtos = _mapper.Map<List<PlaceCardDto>>(places);

            await SetIsSavedStatusAsync(dtos);

            return dtos;
        }

        public async Task<IReadOnlyList<PlaceCardDto>> GetPlacesByCityAsync(int cityId, int page, int pageSize)
        {
            if (cityId <= 0)
                throw new ArgumentException("Invalid city ID.");

            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 10;

            var places = await _unitOfWork.Places.GetByCityAsync(cityId, page, pageSize);
            var dtos = _mapper.Map<List<PlaceCardDto>>(places);

            await SetIsSavedStatusAsync(dtos);

            return dtos;
        }
    }
}
