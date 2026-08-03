using System;
using System.Collections.Generic;
using System.Text;
using TourEgypt.Core.Entities;

namespace TourEgypt.Core.Interfaces.Repositories
{
    public interface ICityRepository:IGenericRepository<City>
    {
        Task<IReadOnlyList<City>> GetCategoriesAsync(int count);
    }
}
