using System;

namespace RentAPlace.Api.Models
{
    public class Review : BaseEntity
    {
        public int Id { get; set; }
        public int PropertyId { get; set; }
        public Guid RenterId { get; set; }
        public int Rating { get; set; } // 1..5
        public string? Comment { get; set; }
        public Property? Property { get; set; }
    }
}
