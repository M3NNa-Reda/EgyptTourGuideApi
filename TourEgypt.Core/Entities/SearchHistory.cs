using System;
using System.Collections.Generic;
using System.Text;

namespace TourEgypt.Core.Entities
{
    public class SearchHistory
    {
        public int SearchHistoryId { get; set; }
        public string SearchText { get; set; } = string.Empty;
        public DateTime SearchDate { get; set; } = DateTime.UtcNow;
        public int UserId { get; set; }
        public ApplicationUser User { get; set; } = null!;
        public int? PlaceId { get; set; }
        public Place? Place { get; set; }
        public int? CityId { get; set; }
        public City? City { get; set; }
        public int? CategoryId { get; set; }
        public Category? Category { get; set; }



    }
}
