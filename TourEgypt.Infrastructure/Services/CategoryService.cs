using System;
using System.Collections.Generic;
using System.Text;
using TourEgypt.Core.DTOs.Category;
using TourEgypt.Core.Entities;
using TourEgypt.Core.Interfaces.Repositories;
using TourEgypt.Core.Interfaces.Services;

namespace TourEgypt.Infrastructure.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public IEnumerable<CategoryDto> GetCategories()
        {
            var categories = _categoryRepository.GetAllAsync().Result;
            return categories.Select(c => new CategoryDto
            {
                CategoryId = c.CategoryId,
                Name = c.Name,
                IconUrl = c.IconUrl
            });
    }
    }
}