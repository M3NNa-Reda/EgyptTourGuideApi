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
        public async Task<IEnumerable<CityDto>> GetAllCitiesAsync()
        {
            var cities = await _unitOfWork.Cities.GetAllAsync();
            return _mapper.Map<IEnumerable<CityDto>>(cities);
        }

        public async Task<CityDto?> GetCityByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid city ID.");

            var city = await _unitOfWork.Cities.GetByIdAsync(id);

            if (city == null)
                throw new KeyNotFoundException("City not found.");

            return _mapper.Map<CityDto>(city);
        }
        public async Task UpdateCityMetricsAsync()
        {
            var cities = await _unitOfWork.Cities.GetAllWithPlacesAndReviewsAsync();

            foreach (var city in cities)
            {
                if (city.Places == null || !city.Places.Any())
                {
                    city.ReviewsCount = 0;
                    city.AverageRating = 0;
                    continue;
                }
                var allReviews = city.Places
                    .Where(p => p.Reviews != null)
                    .SelectMany(p => p.Reviews);

                city.ReviewsCount = allReviews.Count();
                city.AverageRating = allReviews.Any()
                    ? Math.Round(allReviews.Average(r => r.Rating), 1)
                    : 0;

            }

            await _unitOfWork.CompleteAsync();  
        }
        public async Task<int> CreateCityAsync(CityDto createDto)
        {
            if (string.IsNullOrWhiteSpace(createDto.Name))
                throw new ArgumentException("City name is required.");

            var cities = await _unitOfWork.Cities.GetAllAsync();
            var isExist = cities.Any(c => c.Name.Trim().Equals(createDto.Name.Trim(), StringComparison.OrdinalIgnoreCase));

            if (isExist)
                throw new InvalidOperationException("A city with this name already exists.");
            var cityEntity = _mapper.Map<City>(createDto);
            await _unitOfWork.Cities.AddAsync(cityEntity);
            await _unitOfWork.CompleteAsync();
            return cityEntity.CityId;


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

        public async Task DeleteCityAsync(int id)
        {
            var city = await _unitOfWork.Cities.GetByIdAsync(id);

            if (city == null)
                throw new KeyNotFoundException("City not found");


            _unitOfWork.Cities.Delete(city);

            await _unitOfWork.CompleteAsync();
        }
    }
}
