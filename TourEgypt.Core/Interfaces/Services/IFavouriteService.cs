using System;
using System.Collections.Generic;
using System.Text;
using TourEgypt.Core.DTOs.Place;

namespace TourEgypt.Core.Interfaces.Services
{
    public interface IFavouriteService
    {
        Task AddFavouriteAsync(int placeId);
        Task RemoveFavouriteAsync(int placeId);
        Task<IEnumerable<PlaceCardDto>> GetAllFavouritesAsync();
        Task<bool> IsFavouriteAsync(int placeId);
    }
}
