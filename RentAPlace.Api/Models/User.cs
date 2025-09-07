using System;

namespace RentAPlace.Api.Models
{
    // Application user: Role is either Renter or Owner
    public class User : BaseEntity
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set; } = "Renter";
        public string? Phone { get; set; }
    }
}
