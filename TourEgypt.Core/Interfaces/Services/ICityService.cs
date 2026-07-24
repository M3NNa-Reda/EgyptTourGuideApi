using System;
using System.Collections.Generic;
using System.Text;
using TourEgypt.Core.DTOs.City;

namespace TourEgypt.Core.Interfaces.Services
{
    public interface ICityService
    {
        Task<List<CityDto>> GetPopularCitiesAsync();

    }
}
