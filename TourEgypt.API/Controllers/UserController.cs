using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourEgypt.Core.DTOs.Auth;
using TourEgypt.Core.DTOs.User;
using TourEgypt.Core.Interfaces.Services;
using TourEgypt.Infrastructure.Services;

namespace TourEgypt.API.Controllers
{
    [Authorize] 
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            try
            {
                var result = await _userService.GetProfileAsync();
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto dto)
        {
            try
            {
                await _userService.UpdateProfileAsync(dto);
                return Ok(new { message = "Profile updated successfully." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("profile-image")]
        public async Task<IActionResult> UpdateProfileImage(IFormFile image)
        {
            try
            {
                await _userService.UpdateProfileImageAsync(image);
                return Ok(new { message = "Profile image updated successfully." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpDelete("profile-image")]
        public async Task<IActionResult> DeleteProfileImage()
        {
            try
            {
                await _userService.DeleteProfileImageAsync();
                return Ok(new { message = "Profile image deleted successfully." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPost("interests")]
        public async Task<IActionResult> SaveUserInterests([FromBody] List<int> interestIds)
        {
            try
            {
                await _userService.SaveUserInterestsAsync(interestIds);
                return Ok(new { message = "User interests saved successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
