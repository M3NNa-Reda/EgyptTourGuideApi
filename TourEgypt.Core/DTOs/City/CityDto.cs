using System;
using System.Collections.Generic;
using System.Text;

namespace TourEgypt.Core.DTOs.City
{
    public class CityDto
    {
        public int CityId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int ReviewsCount { get; set; }
        public double AverageRating { get; set; }
    }
}
