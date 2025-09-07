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
    public class ReviewsController : ControllerBase
    {
        private readonly AppDbContext _db;

        public ReviewsController(AppDbContext db)
        {
            _db = db;
        }

        // GET api/reviews/property/{id}
        // Anyone can see reviews for a property
        [HttpGet("property/{propertyId}")]
        public async Task<ActionResult> GetByProperty(int propertyId)
        {
            var reviews = await (
            from r in _db.Reviews
            join u in _db.Users on r.RenterId equals u.Id
            where r.PropertyId == propertyId
            select new
            {
                r.Id,
                r.RenterId,
                RenterName = u.FullName,
                r.Rating,
                r.Comment,
                r.CreatedAt
            }).ToListAsync();

            return Ok(reviews);
        }

        // POST api/reviews
        // Only renters can leave a review
        [HttpPost]
        [Authorize(Roles = "Renter")]
        public async Task<ActionResult> Create(Review review)
        {
            var sub = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(sub)) return Unauthorized();
            var renterId = Guid.Parse(sub);

            // Validate: comment cannot exist without rating
            if (review.Comment != null && review.Rating <= 0)
            {
                return BadRequest("Rating is required if comment is provided");
            }

            // Validate: one review per renter per property
            var existing = await _db.Reviews.FirstOrDefaultAsync(r =>
                r.PropertyId == review.PropertyId && r.RenterId == renterId);
            if (existing != null)
            {
                return BadRequest("You have already reviewed this property");
            }

            // Assign renter id
            review.RenterId = renterId;
            review.CreatedAt = DateTime.UtcNow;

            _db.Reviews.Add(review);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetByProperty), new { propertyId = review.PropertyId }, review);
        }
    }
}
