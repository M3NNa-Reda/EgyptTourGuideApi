using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourEgypt.Core.DTOs.Category;
using TourEgypt.Core.Interfaces.Services;

namespace TourEgypt.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> GetAllCategories()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            return Ok(categories);
        }

        [HttpGet("top/{count:int}")]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> GetCategories(int count=5)
        {
            var categories = await _categoryService.GetCategoriesAsync(count);
            return Ok(categories);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<int>> CreateCategory([FromBody] CategoryDto createDto)
        {
            var categoryId = await _categoryService.CreateCategoryAsync(createDto);
            return StatusCode(StatusCodes.Status201Created, new { id = categoryId, message = "Category created successfully." });
        }
        [Authorize(Roles = "Admin")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] CategoryDto updateDto)
        {
            await _categoryService.UpdateCategoryAsync(id, updateDto);
            return Ok(new { message = "Category updated successfully." });
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            await _categoryService.DeleteCategoryAsync(id);
            return Ok(new { message = "Category deleted successfully." });
        }
    }
}