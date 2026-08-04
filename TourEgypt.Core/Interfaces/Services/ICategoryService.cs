using System;
using System.Collections.Generic;
using System.Text;
using TourEgypt.Core.DTOs.Category;

namespace TourEgypt.Core.Interfaces.Services
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDto>> GetCategoriesAsync(int count);
        Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync();


        Task<int> CreateCategoryAsync(CategoryDto createDto);

        Task UpdateCategoryAsync(int id, CategoryDto updateDto);

        Task DeleteCategoryAsync(int id);
    }
}
