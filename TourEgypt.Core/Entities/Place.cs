using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;
using System.Text;

namespace TourEgypt.Core.Entities
{
    public class Place
    {
        public int PlaceId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int? DurationInDays { get; set; }
        public string? CuisineType { get; set; }
        public double AverageRating { get; set; }
        public int ReviewsCount { get; set; }
        public int? FavoriteCount { get; set; }

        public int CityId { get; set; }
        public int CategoryId { get; set; }
        public City City { get; set; } = null!;
        public Category Category { get; set; } = null!;

        public ICollection<Review> Reviews { get; set; } = new List<Review>();

        public ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();



    }
}
