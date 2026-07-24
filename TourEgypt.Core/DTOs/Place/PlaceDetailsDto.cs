using System;
using System.Collections.Generic;
using System.Text;

namespace TourEgypt.Core.DTOs.Place
{
    public class PlaceDetailsDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public int? DurationInDays { get; set; }
        public string? CuisineType { get; set; }
        public double AverageRating { get; set; }
        public int ReviewsCount { get; set; }
        public bool IsSaved { get; set; }
    }
}
