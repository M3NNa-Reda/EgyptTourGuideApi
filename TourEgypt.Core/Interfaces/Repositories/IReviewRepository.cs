using System;
using System.Collections.Generic;
using System.Text;
using TourEgypt.Core.Entities;

namespace TourEgypt.Core.Interfaces.Repositories
{
    public interface IReviewRepository : IGenericRepository<Review>
    {
        Task<IReadOnlyList<Review>> GetByPlaceIdAsync(int placeId, int page, int pageSize);
        Task<int> CountByUserIdAsync(int userId);
    }
}
