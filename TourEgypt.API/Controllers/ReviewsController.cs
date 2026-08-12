using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourEgypt.Core.DTOs.Review;
using TourEgypt.Core.Interfaces.Services;

namespace TourEgypt.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewsController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpGet("place/{placeId:int}")]
        public async Task<IActionResult> GetReviewsByPlace(int placeId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var reviews = await _reviewService.GetReviewsByPlaceIdAsync(placeId, page, pageSize);
            return Ok(reviews);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddReview([FromBody] CreateReviewDto dto)
        {
            var reviewId = await _reviewService.AddReviewAsync(dto);
            return StatusCode(StatusCodes.Status201Created, new { id = reviewId, message = "Review added successfully." });
        }

        [Authorize]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteReview(int id)
        {
            await _reviewService.DeleteReviewAsync(id);
            return Ok(new { message = "Review deleted successfully." });
        }
    }
}
