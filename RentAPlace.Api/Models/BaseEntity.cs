using System;

namespace RentAPlace.Api.Models
{
    // Base entity to hold audit columns
    public abstract class BaseEntity
    {
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}