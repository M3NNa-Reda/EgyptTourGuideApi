using System;
using System.Collections.Generic;
using System.Text;

namespace TourEgypt.Core.DTOs.Place
{
    public class NearbyPlaceDto
    {
        public int Id { get; set; }
        public int PlaceId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public double AverageRating { get; set; }
        public int ReviewsCount { get; set; }
        public bool IsSaved { get; set; }
        public double DistanceInKm { get; set; }
    }
}
