using AutoMapper;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using TourEgypt.Core.DTOs.Category;
using TourEgypt.Core.DTOs.Place;
using TourEgypt.Core.Entities;
using TourEgypt.Core.Interfaces.Repositories;
using TourEgypt.Core.Interfaces.Services;

namespace TourEgypt.Infrastructure.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;


        public CategoryService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        async Task<IEnumerable<CategoryDto>> ICategoryService.GetCategoriesAsync(int count)
        {
            var categories = await _unitOfWork.Categories.GetCategoriesAsync(count);

            return _mapper.Map<IEnumerable<CategoryDto>>(categories);
        }
        async Task<IEnumerable<CategoryDto>> ICategoryService.GetAllCategoriesAsync()
        {
            var categories = await _unitOfWork.Categories.GetAllAsync();

            return _mapper.Map<IEnumerable<CategoryDto>>(categories);
        }
        

        async Task<int> ICategoryService.CreateCategoryAsync(CategoryDto createDto)
        {
            var categoryEntity = _mapper.Map<Category>(createDto);
            await _unitOfWork.Categories.AddAsync(categoryEntity);
            await _unitOfWork.CompleteAsync();
            return categoryEntity.CategoryId;


        }
        async Task ICategoryService.UpdateCategoryAsync(int id, CategoryDto updateDto)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id);

            if (category == null)
                throw new KeyNotFoundException("Category not found");

            _mapper.Map(updateDto, category);

            _unitOfWork.Categories.Update(category);

            await _unitOfWork.CompleteAsync();
        }


        async Task ICategoryService.DeleteCategoryAsync(int id)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id);

            if (category == null)
                throw new KeyNotFoundException("Category not found");


            _unitOfWork.Categories.Delete(category);

            await _unitOfWork.CompleteAsync();
        }

        
    }
}