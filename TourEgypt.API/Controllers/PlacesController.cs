using Microsoft.AspNetCore.Mvc;
using TourEgypt.Core.DTOs.Place;
using TourEgypt.Core.Interfaces.Services;

namespace TourEgypt.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlacesController : ControllerBase
    {
        private readonly IPlaceService _placeService;

        public PlacesController(IPlaceService placeService)
        {
            _placeService = placeService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var place = await _placeService.GetPlaceByIdAsync(id);
            if (place == null)
                return NotFound(new { message = "Place not found" });

            return Ok(place);
        }

        [HttpGet("category/{categoryId}")]
        public async Task<IActionResult> GetByCategory(int categoryId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var places = await _placeService.GetPlacesByCategoryAsync(categoryId, page, pageSize);
            return Ok(places);
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string keyword, [FromQuery] int count = 5)
        {
            var places = await _placeService.SearchPlacesAsync(keyword, count);
            return Ok(places);
        }

        [HttpGet("nearby")]
        public async Task<IActionResult> GetNearby([FromQuery] double latitude, [FromQuery] double longitude, [FromQuery] double maxDistanceInKm = 10)
        {
            try
            {
                var places = await _placeService.GetNearbyPlacesAsync(latitude, longitude, maxDistanceInKm);
                return Ok(places);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SavePlaceDto placeDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _placeService.CreatePlaceAsync(placeDto);
            return Ok(new { message = "Place created successfully" });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] SavePlaceDto placeDto)
        {
            try
            {
                await _placeService.UpdatePlaceAsync(id, placeDto);
                return Ok(new { message = "Place updated successfully" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _placeService.DeletePlaceAsync(id);
                return Ok(new { message = "Place deleted successfully" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}
