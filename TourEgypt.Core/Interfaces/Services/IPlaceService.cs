using System;
using System.Collections.Generic;
using System.Text;
using TourEgypt.Core.DTOs.Place;

namespace TourEgypt.Core.Interfaces.Services
{
    public interface IPlaceService
    {
        Task<IReadOnlyList<PlaceCardDto>> GetPlacesByCategoryAsync(int categoryId, int page, int pageSize);

        Task<IReadOnlyList<PlaceCardDto>> SearchPlacesAsync(string keyword, int count);

        Task<IReadOnlyList<NearbyPlaceDto>> GetNearbyPlacesAsync(double latitude, double longitude, double maxDistanceInKm);

        Task<PlaceDetailsDto?> GetPlaceByIdAsync(int id);

        Task<int> CreatePlaceAsync(SavePlaceDto placeDto);

        Task UpdatePlaceAsync(int id, SavePlaceDto placeDto);

        Task DeletePlaceAsync(int id);
    }
}
