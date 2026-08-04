using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using TourEgypt.Core.Entities;
using TourEgypt.Core.Interfaces.Repositories;
using TourEgypt.Data.Context;
using TourEgypt.Infrastructure.Repositories;

namespace TourEgypt.Infrastructure.Repositories
{
    public class CityRepository : GenericRepository<City>, ICityRepository
    {
        public CityRepository(AppDbContext context) : base(context)
        {
        }


        public async Task<IEnumerable<City>> GetPopularCitiesAsync(int page, int pageSize)
        {
            return await _context.Cities
                .AsNoTracking()
                .OrderByDescending(c => c.AverageRating)
                .ThenByDescending(c => c.ReviewsCount)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
        public async Task<IEnumerable<City>> GetAllWithPlacesAndReviewsAsync()
        {
            return await _context.Cities
                .Include(c => c.Places)
                .ThenInclude(p => p.Reviews)
                .ToListAsync();
        }

    }
}
