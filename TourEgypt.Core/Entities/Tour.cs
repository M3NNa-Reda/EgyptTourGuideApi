using System;
using System.Collections.Generic;
using System.Text;
using TourEgypt.Core.Entities;

public class Tour
{
    public int TourId { get; set; }

    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;

    public decimal Price { get; set; }
    public int DurationInHours { get; set; }
    public string TourType { get; set; } = string.Empty;

    public double AverageRating { get; set; }
    public int ReviewsCount { get; set; }

    public int PlaceId { get; set; }
    public Place Place { get; set; } = null!;



}