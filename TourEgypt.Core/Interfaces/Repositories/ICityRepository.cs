using System;
using System.Collections.Generic;
using System.Text;
using TourEgypt.Core.Entities;

namespace TourEgypt.Core.Interfaces.Repositories
{
    public interface ICityRepository:IGenericRepository<City>
    {
        Task<IEnumerable<City>> GetAllWithPlacesAndReviewsAsync();
        Task<IEnumerable<City>> GetPopularCitiesAsync(int page, int pageSize);
    }
}
