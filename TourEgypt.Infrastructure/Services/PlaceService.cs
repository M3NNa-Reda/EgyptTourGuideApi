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
using TourEgypt.Infrastructure.Repositories;

namespace TourEgypt.Infrastructure.Services
{
    public class PlaceService : IPlaceService
    {
       private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        
        public PlaceService( IMapper mapper,IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        

        public async Task<IReadOnlyList<NearbyPlaceDto>> GetNearbyPlacesAsync(double latitude, double longitude, double maxDistanceInKm)
        {
            if (latitude < -90 || latitude > 90 ||
                longitude < -180 || longitude > 180)
                throw new ArgumentException("Invalid coordinates.");

            if (maxDistanceInKm <= 0)
                maxDistanceInKm = 10;

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

             return result;
        }

        public async Task<PlaceDetailsDto?> GetPlaceByIdAsync(int id)
        {
            var place = await _unitOfWork.Places.GetByIdAsync(id);
            return _mapper.Map<PlaceDetailsDto>(place);
        }

        public async Task<IReadOnlyList<PlaceCardDto>> GetPlacesByCategoryAsync(int categoryId, int page, int pageSize)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 10;
            var places = await _unitOfWork.Places.GetByCategoryAsync(categoryId, page, pageSize);
            return  _mapper.Map<IReadOnlyList<PlaceCardDto>>(places);
        }

        public async Task<IReadOnlyList<PlaceCardDto>> SearchPlacesAsync(string keyword, int count)
        {
            if (count <= 0) count = 5;
            var places = await _unitOfWork.Places.SearchAsync(keyword, count);
            return _mapper.Map<IReadOnlyList<PlaceCardDto>>(places);
        }
        public async Task CreatePlaceAsync(SavePlaceDto placeDto)
        {
            var placeEntity = _mapper.Map<Place>(placeDto);
            await _unitOfWork.Places.AddAsync(placeEntity);
            await _unitOfWork.CompleteAsync();

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
    }
}
