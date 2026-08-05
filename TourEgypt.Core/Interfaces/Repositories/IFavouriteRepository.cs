using System;
using System.Collections.Generic;
using System.Text;
using TourEgypt.Core.Entities;

namespace TourEgypt.Core.Interfaces.Repositories
{
    public interface IFavouriteRepository : IGenericRepository<Favorite>
    {

        Task<IEnumerable<Favorite>> GetAllByUserIdAsync(int userId);
        Task<bool> IsFavouriteAsync(int userId, int placeId);
        Task<int> CountByUserIdAsync(int userId);
        Task<List<int>> GetUserFavoritePlaceIdsAsync(int userId);
    }
}
