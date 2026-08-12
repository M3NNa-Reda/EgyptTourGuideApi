using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using TourEgypt.Core.Entities;
using TourEgypt.Core.Interfaces.Repositories;
using TourEgypt.Data.Context;

namespace TourEgypt.Infrastructure.Repositories
{
    public class ReviewRepository : GenericRepository<Review>, IReviewRepository
    {
        public ReviewRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<int> CountByUserIdAsync(int userId)
        {
           return await _context.Reviews
                .CountAsync(f => f.UserId == userId);
        }

        public async Task<IReadOnlyList<Review>> GetByPlaceIdAsync(int placeId, int page, int pageSize)
        {
            return await _context.Reviews
                .Include(r => r.User)
                .Where(r => r.PlaceId == placeId)
                .OrderByDescending(r => r.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
