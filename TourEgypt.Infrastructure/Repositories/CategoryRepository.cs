using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using TourEgypt.Core.Entities;
using TourEgypt.Core.Interfaces.Repositories;
using TourEgypt.Data.Context;

namespace TourEgypt.Infrastructure.Repositories
{
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IReadOnlyList<Category>> GetTopCategoriesAsync(int count)
        {
            return await _context.Categories
                .Take(count)
                .ToListAsync();
        }
    }
}