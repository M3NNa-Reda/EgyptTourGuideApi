using System;
using System.Collections.Generic;
using System.Text;

namespace TourEgypt.Core.DTOs.Tour
{
    public class TourDto
    {
        public int TourId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int DurationInHours { get; set; }
        public string TourType { get; set; } = string.Empty;
    }
}
