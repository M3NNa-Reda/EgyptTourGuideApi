using System;
using System.Collections.Generic;
using System.Text;

namespace TourEgypt.Core.DTOs.Review
{
    public class ReviewDto
    {
        public int Id { get; set; }
        public int PlaceId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string? UserProfileImage { get; set; }
        public double Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
