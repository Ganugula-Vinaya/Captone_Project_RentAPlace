using System;
using System.Collections.Generic;

namespace RentAPlace.Api.Models
{
    public class Property : BaseEntity
    {
        public int Id { get; set; }
        public Guid OwnerId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Address { get; set; }
        public string City { get; set; } = string.Empty;
        public string? State { get; set; }
        public string Country { get; set; } = string.Empty;
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public decimal PricePerNight { get; set; }
        public string PropertyType { get; set; } = "Apartment";
        public int MaxGuests { get; set; } = 1;
        public int Bedrooms { get; set; } = 1;
        public int Bathrooms { get; set; } = 1;
        public bool IsActive { get; set; } = true;
        public decimal AverageRating { get; set; } = 0m;

        public User? Owner { get; set; }
        public ICollection<PropertyImage> Images { get; set; } = new List<PropertyImage>();
        public ICollection<PropertyAmenity> PropertyAmenities { get; set; } = new List<PropertyAmenity>();
    }
}
