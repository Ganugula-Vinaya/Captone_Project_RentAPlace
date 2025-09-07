using System.Collections.Generic;

namespace RentAPlace.Api.Dtos
{
    // Property creation payload used by Owner
    public class PropertyCreateDto
    {
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
        public List<int> AmenityIds { get; set; } = new();
    }

    // For updates we reuse the same fields
    public class PropertyUpdateDto : PropertyCreateDto { }

    // Search query used by public listing/search
    public class SearchQuery
    {
        public string? City { get; set; }
        public string? Country { get; set; }
        public string? PropertyType { get; set; }
        public string? FeaturesCsv { get; set; } // Amenity names comma-separated
        public DateOnly? CheckIn { get; set; }
        public DateOnly? CheckOut { get; set; }
        public int? Guests { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 12;
    }
}
