namespace RentAPlace.Api.Models
{
    public class PropertyImage
    {
        public int Id { get; set; }
        public int PropertyId { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public Property? Property { get; set; }
    }
}
