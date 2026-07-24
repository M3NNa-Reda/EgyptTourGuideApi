using System;
using System.Collections.Generic;
using System.Text;

namespace TourEgypt.Core.DTOs.Place
{
    public class PlaceCardDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public double AverageRating { get; set; }
        public bool IsSaved { get; set; }
        public string? CuisineType { get; set; }

    }
}
