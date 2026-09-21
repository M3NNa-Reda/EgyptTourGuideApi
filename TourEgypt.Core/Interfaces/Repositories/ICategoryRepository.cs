using System;
using System.Collections.Generic;
using System.Text;
using TourEgypt.Core.Entities;

namespace TourEgypt.Core.Interfaces.Repositories
{
    public interface ICategoryRepository:IGenericRepository<Category>
    {
        Task<IReadOnlyList<Category>> GetCategoriesAsync(int count);
    }
}
