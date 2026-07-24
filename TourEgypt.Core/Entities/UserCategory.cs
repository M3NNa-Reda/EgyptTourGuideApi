using System;
using System.Collections.Generic;
using System.Text;

namespace TourEgypt.Core.Entities
{
    public class UserCategory
    {
       

        public int UserId { get; set; }
        public int CategoryId { get; set; }
        public ApplicationUser User { get; set; } = null!;
        public Category Category { get; set; } = null!;

    }
}
