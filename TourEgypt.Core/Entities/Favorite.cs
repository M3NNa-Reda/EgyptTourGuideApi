using System;
using System.Collections.Generic;
using System.Text;

namespace TourEgypt.Core.Entities
{
    public class Favorite
    {

        public int UserId { get; set; }
        public ApplicationUser User { get; set; } = new ApplicationUser();
        public int PlaceId { get; set; }
        public Place Place { get; set; } = new Place();
        
    }
}
