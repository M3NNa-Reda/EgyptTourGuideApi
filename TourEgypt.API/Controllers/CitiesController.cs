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

        [HttpGet]
        public async Task<IActionResult> GetAllCitiesAsync()
        {
            var cities = await _cityService.GetAllCitiesAsync();
            return Ok(cities);
        }
        [HttpGet("popular")]
        public async Task<IActionResult> GetPopularCities([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var cities = await _cityService.GetPopularCitiesAsync(page, pageSize);
            return Ok(cities);
        }
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetCityById(int id)
        {
            var city = await _cityService.GetCityByIdAsync(id);
            return Ok(city);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<int>> CreateCity([FromBody] CityDto createDto)
        {
            var cityId = await _cityService.CreateCityAsync(createDto);
            return StatusCode(StatusCodes.Status201Created, new { id = cityId, message = "City created successfully." });
        }
        
        [Authorize(Roles = "Admin")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateCity(int id, [FromBody] CityDto updateDto)
        {
            await _cityService.UpdateCityAsync(id, updateDto);
            return Ok(new { message = "City updated successfully." });
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteCity(int id)
        {
            await _cityService.DeleteCityAsync(id);
            return Ok(new { message = "City deleted successfully." });
        }
    }
}
