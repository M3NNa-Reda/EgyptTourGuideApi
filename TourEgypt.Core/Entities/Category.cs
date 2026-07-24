using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace TourEgypt.Core.Entities
{
    public class Category
    {
        public int CategoryId { get; set; }

        public string Name { get; set; } = string.Empty;
        public string IconUrl { get; set; } = string.Empty;

        public ICollection<Place> Places { get; set; } = new List<Place>();
        public ICollection<UserCategory> UserCategories { get; set; } = new List<UserCategory>();



    }
}
