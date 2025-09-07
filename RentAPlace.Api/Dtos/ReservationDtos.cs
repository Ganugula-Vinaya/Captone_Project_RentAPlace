using System;

namespace RentAPlace.Api.Dtos
{
    // Renter creates a reservation using this payload
    public class ReservationCreateDto
    {
        public int PropertyId { get; set; }
        public DateOnly CheckIn { get; set; }
        public DateOnly CheckOut { get; set; }
        public int Guests { get; set; }
    }
}
