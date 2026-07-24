using System;
using System.Collections.Generic;
using System.Text;
using TourEgypt.Core.DTOs.Category;
using TourEgypt.Core.DTOs.City;
using TourEgypt.Core.DTOs.Place;

namespace TourEgypt.Core.DTOs.Home
{
    public class HomeDataDto
    {
        public List<CategoryDto> Categories { get; set; } = new();
        public List<CityDto> PopularDestinations { get; set; } = new();
        public List<PlaceDetailsDto> TopAttractions { get; set; } = new();
        public List<PlaceDetailsDto> RecommendedTrips { get; set; } = new();
        public List<PlaceDetailsDto> FeaturedHotels { get; set; } = new();
        public List<PlaceDetailsDto> FeaturedRestaurants { get; set; } = new();
    }
}
