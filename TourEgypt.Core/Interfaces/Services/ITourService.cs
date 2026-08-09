using System;
using System.Collections.Generic;
using System.Text;
using TourEgypt.Core.DTOs.Tour;

namespace TourEgypt.Core.Interfaces.Services
{
    public interface ITourService
    {
        Task<IReadOnlyList<TourDto>> GetByPlaceIdAsync(int placeId);
    }
}