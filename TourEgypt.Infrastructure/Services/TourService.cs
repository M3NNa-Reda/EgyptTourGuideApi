using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using TourEgypt.Core.DTOs.Tour;
using TourEgypt.Core.Interfaces.Repositories;
using TourEgypt.Core.Interfaces.Services;

namespace TourEgypt.Infrastructure.Services
{
    public class TourService : ITourService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public TourService(
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IReadOnlyList<TourDto>> GetByPlaceIdAsync(int placeId)
        {
            if (placeId <= 0)
                throw new ArgumentException("Invalid place.");

            var place = await _unitOfWork.Places.GetByIdAsync(placeId);

            if (place == null)
                throw new KeyNotFoundException("Place not found.");

            var tours = await _unitOfWork.Tours.GetByPlaceIdAsync(placeId);

            return _mapper.Map<List<TourDto>>(tours);
        }
    }
}