using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using TourEgypt.Core.Entities;
using TourEgypt.Core.Interfaces.Repositories;
using TourEgypt.Data.Context;
using TourEgypt.Infrastructure.Repositories;
public class TourRepository : GenericRepository<Tour>, ITourRepository
{
    public TourRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<Tour>> GetByPlaceIdAsync(int placeId)
    {
        return await _context.Tours
         .AsNoTracking()
         .Where(t => t.PlaceId == placeId)
         .OrderByDescending(t => t.AverageRating)
         .ThenByDescending(t => t.ReviewsCount)
         .ToListAsync();
    }
}