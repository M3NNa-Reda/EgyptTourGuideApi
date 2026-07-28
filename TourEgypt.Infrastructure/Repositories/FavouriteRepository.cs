using Microsoft.EntityFrameworkCore;
using TourEgypt.Core.Entities;
using TourEgypt.Core.Interfaces.Repositories;
using TourEgypt.Data.Context;

namespace TourEgypt.Infrastructure.Repositories
{
    public class FavouriteRepository : IFavouriteRepository
    {
        private readonly AppDbContext _context;

        public FavouriteRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Favorite favourite)
        {
            await _context.Favorites.AddAsync(favourite);
        }

        public async Task RemoveAsync(Favorite favourite)
        {
            _context.Favorites.Remove(favourite);
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
    }
}