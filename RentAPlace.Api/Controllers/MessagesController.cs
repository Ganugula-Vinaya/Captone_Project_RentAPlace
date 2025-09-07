using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RentAPlace.Api.Data;
using RentAPlace.Api.Models;
using System.Security.Claims;

namespace RentAPlace.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MessagesController : ControllerBase
    {
        private readonly AppDbContext _db;
        public MessagesController(AppDbContext db) { _db = db; }

        // POST api/messages
        [HttpPost]
        [Authorize] // both Renter and Owner can send
        public async Task<ActionResult> Send(Message dto)
        {
            var sub = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(sub)) return Unauthorized();
            var senderId = Guid.Parse(sub);

            var msg = new Message
            {
                PropertyId = dto.PropertyId,
                SenderId = senderId,
                ReceiverId = dto.ReceiverId,
                Content = dto.Content,
                SentAt = DateTime.UtcNow,
                IsRead = false
            };

            _db.Messages.Add(msg);
            await _db.SaveChangesAsync();

            return Ok(new { msg.Id, msg.Content, msg.SentAt });
        }

        // GET api/messages/property/{propertyId}
        [HttpGet("property/{propertyId}")]
        [Authorize]
        public async Task<ActionResult> GetByProperty(int propertyId)
        {
            var sub = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(sub)) return Unauthorized();
            var userId = Guid.Parse(sub);

            //Fetch messages based on role
            // 1. Check if current user is owner of this property
            var property = await _db.Properties.FindAsync(propertyId);
            if (property == null) return NotFound();

            List<Message> msgs;

            if (property.OwnerId == userId)
            {
                // Current user is owner: show messages sent by renters only
                msgs = await _db.Messages
                    .Where(m => m.PropertyId == propertyId && m.SenderId != userId)
                    .OrderBy(m => m.SentAt)
                    .ToListAsync();
            }
            else
            {
                // Current user is renter: show only messages related to this renter
                msgs = await _db.Messages
                    .Where(m => m.PropertyId == propertyId &&
                                (m.SenderId == userId || m.ReceiverId == userId))
                    .OrderBy(m => m.SentAt)
                    .ToListAsync();
            }

            return Ok(msgs);
        }


        // PATCH api/messages/{id}/read
        [HttpPatch("{id}/read")]
        [Authorize]
        public async Task<ActionResult> MarkAsRead(int id)
        {
            var msg = await _db.Messages.FindAsync(id);
            if (msg == null) return NotFound();

            msg.IsRead = true;
            await _db.SaveChangesAsync();

            return Ok(new { msg.Id, msg.IsRead });
        }
    }
}
