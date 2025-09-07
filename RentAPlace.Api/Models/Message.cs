using System;

namespace RentAPlace.Api.Models
{
    public class Message
    {
        public int Id { get; set; }
        public int PropertyId { get; set; }
        public Guid SenderId { get; set; }
        public Guid ReceiverId { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
        public bool IsRead { get; set; }

        public Property? Property { get; set; }
    }
}
