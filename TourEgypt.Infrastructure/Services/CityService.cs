using AutoMapper;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using TourEgypt.Core.DTOs.Category;
using TourEgypt.Core.DTOs.City;
using TourEgypt.Core.DTOs.Place;
using TourEgypt.Core.Entities;
using TourEgypt.Core.Interfaces.Repositories;
using TourEgypt.Core.Interfaces.Services;

namespace TourEgypt.Infrastructure.Services
{
    public class CityService : ICityService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;


        public CityService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
       
        public async Task<IEnumerable<CityDto>> GetPopularCitiesAsync(int page, int pageSize)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 10;
            var cities = await _unitOfWork.Cities.GetPopularCitiesAsync(page, pageSize);
            return _mapper.Map<IEnumerable<CityDto>>(cities);
        }

        public async Task UpdateCityMetricsAsync()
        {
            var cities = await _unitOfWork.Cities.GetAllWithPlacesAndReviewsAsync();

            foreach (var city in cities)
            {
                var allReviews = city.Places
                    .Where(p => p.Reviews != null)
                    .SelectMany(p => p.Reviews)
                    .ToList();

                city.ReviewsCount = allReviews.Count;
                city.AverageRating = allReviews.Any()
                    ? Math.Round(allReviews.Average(r => r.Rating), 1)
                    : 0;

            }

            await _unitOfWork.CompleteAsync();
        }

        public async Task UpdateCityAsync(int id, CityDto updateDto)
        {
            var city = await _unitOfWork.Cities.GetByIdAsync(id);

            if (city == null)
                throw new KeyNotFoundException("City not found");

            _mapper.Map(updateDto, city);

            _unitOfWork.Cities.Update(city);

            await _unitOfWork.CompleteAsync();
        }

        
    }
}
