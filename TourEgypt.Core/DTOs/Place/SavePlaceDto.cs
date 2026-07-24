using System;
using System.Collections.Generic;
using System.Text;

namespace TourEgypt.Core.DTOs.Place
{
    public class SavePlaceDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int? DurationInDays { get; set; }
        public string? CuisineType { get; set; }

        public int CityId { get; set; }
        public int CategoryId { get; set; }
    }
}
