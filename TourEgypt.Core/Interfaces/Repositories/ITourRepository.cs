using System;
using System.Collections.Generic;
using System.Text;
using TourEgypt.Core.Entities;
using TourEgypt.Core.Interfaces.Repositories;

namespace TourEgypt.Core.Interfaces.Repositories
{
    public interface ITourRepository : IGenericRepository<Tour>
    {
        Task<IReadOnlyList<Tour>> GetByPlaceIdAsync(int placeId);
    }
}