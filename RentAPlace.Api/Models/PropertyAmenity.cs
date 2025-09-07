namespace RentAPlace.Api.Models
{
    // Join table for many-to-many between Property and Amenity
    public class PropertyAmenity
    {
        public int PropertyId { get; set; }
        public int AmenityId { get; set; }

        public Property? Property { get; set; }
        public Amenity? Amenity { get; set; }
    }
}
