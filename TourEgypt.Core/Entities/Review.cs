using System;
using System.Collections.Generic;
using System.Text;

namespace TourEgypt.Core.Entities
{
    public class Review
    {

        public int ReviewId { get; set; }
        public string Content { get; set; } = string.Empty;
        public int Rating { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int UserId { get; set; }
        public ApplicationUser User { get; set; } = new ApplicationUser();
        public int PlaceId { get; set; }
        public Place Place { get; set; } = new Place();


    }
}
