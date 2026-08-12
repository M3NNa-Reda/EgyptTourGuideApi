using System;
using System.Collections.Generic;
using System.Text;
using TourEgypt.Core.DTOs.Review;

namespace TourEgypt.Core.Interfaces.Services
{
    public interface IReviewService
    {
        Task<IReadOnlyList<ReviewDto>> GetReviewsByPlaceIdAsync(int placeId, int page, int pageSize);
        Task<int> AddReviewAsync(CreateReviewDto dto);
        Task DeleteReviewAsync(int reviewId);
    }
}
