using Microsoft.EntityFrameworkCore;
using TourEgypt.Core.Entities;
using TourEgypt.Core.Interfaces.Repositories;
using TourEgypt.Data.Context;

namespace TourEgypt.Infrastructure.Repositories
{
    public class FavouriteRepository : GenericRepository<Favorite>, IFavouriteRepository
    {
        public FavouriteRepository(AppDbContext context) : base(context)
        {
        }

       

        public async Task<IEnumerable<Favorite>> GetAllByUserIdAsync(int userId)
        {
            return await _context.Favorites
                .Where(f => f.UserId == userId)
                .Include(f => f.Place)
                .ToListAsync();
        }

        public async Task<bool> IsFavouriteAsync(int userId, int placeId)
        {
            return await _context.Favorites
                .AnyAsync(f => f.UserId == userId && f.PlaceId == placeId);
        }

        public async Task<int> CountByUserIdAsync(int userId)
        {
            return await _context.Favorites
                .CountAsync(f => f.UserId == userId);
        }
        public async Task<List<int>> GetUserFavoritePlaceIdsAsync(int userId)
        {
            return await _context.Favorites
                .Where(f => f.UserId == userId)
                .Select(f => f.PlaceId)
                .ToListAsync();
        }
    }
}