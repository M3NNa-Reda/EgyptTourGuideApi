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
        public async Task<ActionResult<UserProfileDto>> GetProfile()
        {
            var profile = await _userService.GetProfileAsync();

            return Ok(profile);
        }

        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto dto)
        {
            await _userService.UpdateProfileAsync(dto);

            return NoContent();
        }

        [HttpPut("profile-image")]
        public async Task<IActionResult> UpdateProfileImage(IFormFile image)
        {
            await _userService.UpdateProfileImageAsync(image);

            return NoContent();
        }

        [HttpDelete("profile-image")]
        public async Task<IActionResult> DeleteProfileImage()
        {
            await _userService.DeleteProfileImageAsync();

            return NoContent();
        }

        [HttpPost("interests")]
        public async Task<IActionResult> SaveUserInterests([FromBody] List<int> interestIds)
        {
            await _userService.SaveUserInterestsAsync(interestIds);

            return NoContent();
        }
    }
}