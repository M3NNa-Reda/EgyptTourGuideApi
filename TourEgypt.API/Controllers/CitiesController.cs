using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourEgypt.Core.DTOs.City;
using TourEgypt.Core.Interfaces.Services;

namespace TourEgypt.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CitiesController : ControllerBase
    {
        private readonly ICityService _cityService;

        public CitiesController(ICityService cityService)
        {
            _cityService = cityService;
        }

        [HttpGet("popular")]
        public async Task<ActionResult<IEnumerable<CityDto>>> GetPopularCities([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var cities = await _cityService.GetPopularCitiesAsync(page, pageSize);
            return Ok(cities);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateCity(int id, [FromBody] CityDto updateDto)
        {
            await _cityService.UpdateCityAsync(id, updateDto);
            return Ok(new { message = "City updated successfully." });
        }
    }
}
