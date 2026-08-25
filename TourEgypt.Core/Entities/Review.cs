using System;
using System.Collections.Generic;
using System.Text;

namespace TourEgypt.Core.Entities
{
    public class Review
    {

        public int ReviewId { get; set; }
        public string? Comment { get; set; }
        public int Rating { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int UserId { get; set; }
        public ApplicationUser User { get; set; } = null!;
        public int PlaceId { get; set; }
        public Place Place { get; set; } = null!;
      


    }
}
