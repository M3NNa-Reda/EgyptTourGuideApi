using System;
using System.Collections.Generic;
using System.Text;
using TourEgypt.Core.DTOs.Category;

namespace TourEgypt.Core.Interfaces.Services
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDto>> GetCategoriesAsync();
    }
}
