using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using TourEgypt.Core.Entities;
using TourEgypt.Core.Interfaces.Repositories;
using TourEgypt.Data.Context;

namespace TourEgypt.Infrastructure.Repositories
{
    public class PlaceRepository : GenericRepository<Place>, IPlaceRepository
    {
        public PlaceRepository(AppDbContext context) : base(context)
        {

        }



        public async Task<IReadOnlyList<Place>> GetNearbyAsync(double latitude, double longitude, double maxDistanceInKm = 10)
        {
            double latRange = maxDistanceInKm / 111.0;
            double lngRange = maxDistanceInKm / (111.0 * Math.Cos(latitude * (Math.PI / 180)));

           return await _context.Places
                .AsNoTracking()
                .Where(p => p.Latitude >= latitude - latRange && p.Latitude <= latitude + latRange &&
                       p.Longitude >= longitude - lngRange && p.Longitude <= longitude + lngRange)
                .ToListAsync();
            
        }

            
        
                
        public async Task<IReadOnlyList<Place>> SearchAsync(string keyword, int count)
        {

            return await _context.Places
                .AsNoTracking()
                .Where(p => p.Name.Contains(keyword) || p.Description.Contains(keyword))
                .Take(count)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<Place>> GetByCategoryAsync(int categoryId, int page, int pageSize)
        {
            return await _context.Places
                .AsNoTracking()
                .Where(p=>p.CategoryId == categoryId)
                .OrderByDescending(p => p.AverageRating)
                .ThenByDescending(p => p.ReviewsCount)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<Place>> GetAllPlacesWithReviewsAsync()
        {
            return await _context.Places
                .Include(p => p.Reviews)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<Place>> GetTopPlacesAsync(int page, int pageSize)
        {
            return await _context.Places
                .AsNoTracking()
                .OrderByDescending(p => p.AverageRating)
                .ThenByDescending(p => p.ReviewsCount)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<Place>> GetByCityAsync(int cityId, int page, int pageSize)
        {
            return await _context.Places
                .AsNoTracking()
                .Where(p => p.CityId == cityId)
                .OrderByDescending(p => p.AverageRating)
                .ThenByDescending(p => p.ReviewsCount)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
    }
}
