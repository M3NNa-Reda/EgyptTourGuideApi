using System;
using System.Collections.Generic;
using System.Text;
using TourEgypt.Core.DTOs.Tour;

namespace TourEgypt.Core.Interfaces.Services
{
    public interface ITourService
    {
        Task<IReadOnlyList<TourDto>> GetByPlaceIdAsync(int placeId);

        Task<int> CreateTourAsync(CreateTourDto tourDto);

        Task UpdateTourAsync(int id, CreateTourDto tourDto);

        Task DeleteTourAsync(int id);


    }
}