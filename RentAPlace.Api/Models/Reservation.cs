using System;

namespace RentAPlace.Api.Models
{
    public class Reservation : BaseEntity
    {
        public int Id { get; set; }
        public int PropertyId { get; set; }
        public Guid RenterId { get; set; }
        public DateOnly CheckIn { get; set; }
        public DateOnly CheckOut { get; set; }
        public int Guests { get; set; }
        public string Status { get; set; } = "Pending"; // Pending, Confirmed, Cancelled
        public decimal TotalPrice { get; set; }

        public Property? Property { get; set; }
    }
}
