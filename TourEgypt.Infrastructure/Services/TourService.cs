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

        // POST - Admin creates a tour
        public async Task<int> CreateTourAsync(CreateTourDto tourDto)
        {
            if (tourDto.PlaceId <= 0)
                throw new ArgumentException("Invalid place.");

            var place = await _unitOfWork.Places.GetByIdAsync(tourDto.PlaceId);

            if (place == null)
                throw new KeyNotFoundException("Place not found.");

            var tour = _mapper.Map<Tour>(tourDto);

            await _unitOfWork.Tours.AddAsync(tour);
            await _unitOfWork.CompleteAsync();

            return tour.TourId;
        }

        // PUT - Admin updates a tour
        public async Task UpdateTourAsync(int id, CreateTourDto tourDto)
        {
            var tour = await _unitOfWork.Tours.GetByIdAsync(id);

            if (tour == null)
                throw new KeyNotFoundException("Tour not found.");

            if (tourDto.PlaceId <= 0)
                throw new ArgumentException("Invalid place.");

            var place = await _unitOfWork.Places.GetByIdAsync(tourDto.PlaceId);

            if (place == null)
                throw new KeyNotFoundException("Place not found.");

            _mapper.Map(tourDto, tour);

            _unitOfWork.Tours.Update(tour);

            await _unitOfWork.CompleteAsync();
        }

        // DELETE - Admin deletes a tour
        public async Task DeleteTourAsync(int id)
        {
            var tour = await _unitOfWork.Tours.GetByIdAsync(id);

            if (tour == null)
                throw new KeyNotFoundException("Tour not found.");

            _unitOfWork.Tours.Delete(tour);

            await _unitOfWork.CompleteAsync();
        }
    }
}