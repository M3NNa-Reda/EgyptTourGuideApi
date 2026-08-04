using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using TourEgypt.Core.DTOs.Category;
using TourEgypt.Core.DTOs.City;
using TourEgypt.Core.DTOs.Place;
using TourEgypt.Core.DTOs.User;
using TourEgypt.Core.Entities;

namespace TourEgypt.Infrastructure.Mapping
{
    public class TourEgyptProfile : Profile
    {
        public TourEgyptProfile()
        {
            // Place
            CreateMap<Place, PlaceCardDto>();
            CreateMap<Place, PlaceDetailsDto>();
            CreateMap<Place, NearbyPlaceDto>()
                .ForMember(dest => dest.DistanceInKm, opt => opt.Ignore());
            CreateMap<SavePlaceDto, Place>();

            // Category
            CreateMap<Category, CategoryDto>().ReverseMap();

            // City
            CreateMap<City, CityDto>();
            //ApplicationUser
            CreateMap<ApplicationUser, UserDto>()
            .ForMember(dest => dest.FullName,
                opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"));

            CreateMap<ApplicationUser, UserProfileDto>()
             .ForMember(dest => dest.FullName,
                 opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"))
             .ForMember(dest => dest.SavedPlacesCount,
                 opt => opt.MapFrom(src => src.Favorites.Count))
             .ForMember(dest => dest.ReviewsCount,
                 opt => opt.MapFrom(src => src.Reviews.Count));

            // Review
            //CreateMap<Review, ReviewDto>();

            //// User
            //CreateMap<User, UserDto>();
        }
    }
}

