using System;
using System.Collections.Generic;
using System.Text;
using TourEgypt.Core.DTOs.Category;
using TourEgypt.Core.DTOs.City;

namespace TourEgypt.Core.Interfaces.Services
{
    public interface ICityService
    {
        Task<IEnumerable<CityDto>> GetPopularCitiesAsync(int page, int pageSize);
        Task UpdateCityMetricsAsync();
        Task UpdateCityAsync(int id, CityDto updateDto);

    }
}
