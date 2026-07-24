using System;
using System.Collections.Generic;
using System.Text;

namespace TourEgypt.Core.DTOs.Category
{
    public class CategoryDto
    {
        public int CategoryId { get; set; }

        public string Name { get; set; } = string.Empty;
        public string IconUrl { get; set; } = string.Empty;
    }
}
