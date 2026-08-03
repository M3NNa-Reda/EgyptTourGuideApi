using System;
using System.Collections.Generic;
using System.Text;
using TourEgypt.Core.Entities;

namespace TourEgypt.Core.Interfaces.Repositories
{
    public interface IFavouriteRepository
    {
        Task AddAsync(Favorite favourite);
        Task RemoveAsync(Favorite favourite);
        Task<IEnumerable<Favorite>> GetAllByUserIdAsync(int userId);
        Task<bool> IsFavouriteAsync(int userId, int placeId);
        Task<int> CountByUserIdAsync(int userId);
    }
}
