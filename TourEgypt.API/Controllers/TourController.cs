using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourEgypt.Core.DTOs.Tour;
using TourEgypt.Core.Interfaces.Services;

namespace TourEgypt.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TourController : ControllerBase
    {
        private readonly ITourService _tourService;

        public TourController(ITourService tourService)
        {
            _tourService = tourService;
        }

        [HttpGet("place/{placeId}")]
        public async Task<IActionResult> GetByPlaceId(int placeId)
        {
            var tours = await _tourService.GetByPlaceIdAsync(placeId);

            return Ok(tours);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(CreateTourDto dto)
        {
            var tourId = await _tourService.CreateTourAsync(dto);

            return Ok(new { id = tourId });
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, CreateTourDto dto)
        {
            await _tourService.UpdateTourAsync(id, dto);

            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _tourService.DeleteTourAsync(id);

            return NoContent();
        }
    }
}