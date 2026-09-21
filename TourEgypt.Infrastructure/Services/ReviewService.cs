using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TourEgypt.Core.DTOs.Review;
using TourEgypt.Core.Entities;
using TourEgypt.Core.Interfaces.Repositories;
using TourEgypt.Core.Interfaces.Services;

namespace TourEgypt.Infrastructure.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;

        public ReviewService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _currentUserService = currentUserService;
        }

        private int GetUserId()
        {
            var userId = _currentUserService.UserId;
            if (!userId.HasValue)
                throw new UnauthorizedAccessException("User is not authenticated.");
            return userId.Value;
        }

        public async Task<IReadOnlyList<ReviewDto>> GetReviewsByPlaceIdAsync(int placeId, int page, int pageSize)
        {
            if (placeId <= 0)
                throw new ArgumentException("Invalid place ID.");

            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 10;

            var reviews = await _unitOfWork.Reviews.GetByPlaceIdAsync(placeId, page, pageSize);
            return _mapper.Map<List<ReviewDto>>(reviews);
        }

        public async Task<int> AddReviewAsync(CreateReviewDto dto)
        {
            var userId = GetUserId();

            var place = await _unitOfWork.Places.GetByIdAsync(dto.PlaceId);
            if (place == null)
                throw new KeyNotFoundException("Place not found.");

            var reviewEntity = new Review
            {
                PlaceId = dto.PlaceId,
                UserId = userId,
                Rating = dto.Rating,
                Comment = dto.Comment,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Reviews.AddAsync(reviewEntity);

            place.ReviewsCount += 1;

            var currentTotalRating = place.AverageRating * (place.ReviewsCount - 1);
            place.AverageRating = Math.Round((currentTotalRating + dto.Rating) / place.ReviewsCount, 1);


            await _unitOfWork.CompleteAsync();

            return reviewEntity.ReviewId;
        }

        public async Task DeleteReviewAsync(int reviewId)
        {
            var userId = GetUserId();

            var review = await _unitOfWork.Reviews.GetByIdAsync(reviewId);
            if (review == null)
                throw new KeyNotFoundException("Review not found.");

            if (review.UserId != userId)
                throw new UnauthorizedAccessException("You are not authorized to delete this review.");

            var place = await _unitOfWork.Places.GetByIdAsync(review.PlaceId);

            _unitOfWork.Reviews.Delete(review);

            if (place != null && place.ReviewsCount > 0)
            {
                place.ReviewsCount -= 1;
                if (place.ReviewsCount == 0)
                {
                    place.AverageRating = 0;
                }
                else
                {
                    var currentTotalRating = place.AverageRating * (place.ReviewsCount + 1);
                    place.AverageRating = Math.Round((currentTotalRating - review.Rating) / place.ReviewsCount, 1);
                }
            }

            await _unitOfWork.CompleteAsync();
        }
    }
}