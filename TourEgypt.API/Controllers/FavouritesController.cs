using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourEgypt.Core.Interfaces.Services;

namespace TourEgypt.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class FavouritesController : ControllerBase
    {
        private readonly IFavouriteService _favouriteService;

        public FavouritesController(IFavouriteService favouriteService)
        {
            _favouriteService = favouriteService;
        }

        [HttpPost("{placeId}")]
        public async Task<IActionResult> AddFavourite(int placeId)
        {
            await _favouriteService.AddFavouriteAsync(placeId);
            return Ok(new { message = "Added to favourites" });
        }

        [HttpDelete("{placeId}")]
        public async Task<IActionResult> RemoveFavourite(int placeId)
        {
            await _favouriteService.RemoveFavouriteAsync(placeId);
            return Ok(new { message = "Removed from favourites" });
        }

        [HttpGet]
        public async Task<IActionResult> GetAllFavourites()
        {
            var favourites = await _favouriteService.GetAllFavouritesAsync();
            return Ok(favourites);
        }

        [HttpGet("{placeId}/is-favourite")]
        public async Task<IActionResult> IsFavourite(int placeId)
        {
            var result = await _favouriteService.IsFavouriteAsync(placeId);
            return Ok(new { isFavourite = result });
        }
    }
}