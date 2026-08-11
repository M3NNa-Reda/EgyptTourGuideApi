using Microsoft.AspNetCore.Authorization;
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
            var places = await _placeService.GetNearbyPlacesAsync(latitude, longitude, maxDistanceInKm);
            return Ok(places);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SavePlaceDto placeDto)
        {

            var id = await _placeService.CreatePlaceAsync(placeDto);
            return CreatedAtAction(
            nameof(GetById),
            new { id },
            null);
        }
        [Authorize(Roles = "Admin")]

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] SavePlaceDto placeDto)
        {
            await _placeService.UpdatePlaceAsync(id, placeDto);
            return NoContent();


        }
        [Authorize(Roles = "Admin")]

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _placeService.DeletePlaceAsync(id);
            return NoContent();
        }

        [HttpGet("top")]
        public async Task<IActionResult> GetTopPlaces([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var places = await _placeService.GetTopPlacesAsync(page, pageSize);
            return Ok(places);
        }

        [HttpGet("city/{cityId}")]
        public async Task<IActionResult> GetByCity(int cityId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var places = await _placeService.GetPlacesByCityAsync(cityId, page, pageSize);
            return Ok(places);
        }
    }
}
