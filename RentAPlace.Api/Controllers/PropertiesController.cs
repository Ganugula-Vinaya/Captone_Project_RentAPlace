using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RentAPlace.Api.Data;
using RentAPlace.Api.Dtos;
using RentAPlace.Api.Models;
using RentAPlace.Api.Services;
using System.Security.Claims;

namespace RentAPlace.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PropertiesController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IImageService _images;
        public PropertiesController(AppDbContext db, IImageService images) { _db = db; _images = images; }

        // GET api/properties/search?City=...&Country=...&Guests=...
        [HttpGet("search")]
        public async Task<ActionResult<object>> Search([FromQuery] SearchQuery q)
        {
            var query = _db.Properties
                .Include(p => p.Images)
                .Include(p => p.PropertyAmenities).ThenInclude(pa => pa.Amenity)
                .Where(p => p.IsActive)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(q.City)) query = query.Where(p => p.City == q.City);
            if (!string.IsNullOrWhiteSpace(q.Country)) query = query.Where(p => p.Country == q.Country);
            if (!string.IsNullOrWhiteSpace(q.PropertyType)) query = query.Where(p => p.PropertyType == q.PropertyType);

            if (!string.IsNullOrWhiteSpace(q.FeaturesCsv))
            {
                var wanted = q.FeaturesCsv.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                                          .ToHashSet(StringComparer.OrdinalIgnoreCase);
                query = query.Where(p => p.PropertyAmenities.Any(pa => wanted.Contains(pa.Amenity!.Name)));
            }

            if (q.CheckIn.HasValue && q.CheckOut.HasValue)
            {
                var ci = q.CheckIn.Value; var co = q.CheckOut.Value;
                query = query.Where(p => !_db.Reservations.Any(r => r.PropertyId == p.Id && r.Status != "Cancelled" &&
                    (ci < r.CheckOut && co > r.CheckIn)));
            }

            if (q.Guests.HasValue) query = query.Where(p => p.MaxGuests >= q.Guests.Value);

            var total = await query.CountAsync();
            var items = await query
                .OrderByDescending(p => p.AverageRating)
                .Skip((q.Page - 1) * q.PageSize)
                .Take(q.PageSize)
                .Select(p => new
                {
                    p.Id,
                    p.Title,
                    p.City,
                    p.Country,
                    p.PricePerNight,
                    p.PropertyType,
                    p.AverageRating,
                    Images = p.Images.Select(i => i.ImageUrl).Take(5)
                })
                .ToListAsync();

            return Ok(new { total, page = q.Page, pageSize = q.PageSize, items });
        }

        // GET api/properties/5
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> Get(int id)
        {
            var p = await _db.Properties
                .Include(p => p.Images)
                .Include(p => p.PropertyAmenities).ThenInclude(pa => pa.Amenity)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (p == null) return NotFound();
            return Ok(p);
        }

        // POST api/properties
        // Owner creates a property
        [HttpPost]
        [Authorize(Roles = "Owner")]
        public async Task<ActionResult> Create(PropertyCreateDto dto)
        {
            var sub = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(sub)) return Unauthorized();
            var ownerId = Guid.Parse(sub);


            // Check if the property already exists
            var exists = await _db.Properties.AnyAsync(p => p.OwnerId == ownerId && p.Title == dto.Title && p.City == dto.City);
            if (exists)
            {
                return BadRequest("Property with the same name already exists in this city.");
            }



            var prop = new Property
            {
                OwnerId = ownerId,
                Title = dto.Title,
                Description = dto.Description,
                Address = dto.Address,
                City = dto.City,
                State = dto.State,
                Country = dto.Country,
                Latitude = dto.Latitude,
                Longitude = dto.Longitude,
                PricePerNight = dto.PricePerNight,
                PropertyType = dto.PropertyType,
                MaxGuests = dto.MaxGuests,
                Bedrooms = dto.Bedrooms,
                Bathrooms = dto.Bathrooms
            };

            _db.Properties.Add(prop);
            await _db.SaveChangesAsync();

            if (dto.AmenityIds.Any())
            {
                var toAdd = dto.AmenityIds.Select(aid => new PropertyAmenity { PropertyId = prop.Id, AmenityId = aid });
                _db.PropertyAmenities.AddRange(toAdd);
                await _db.SaveChangesAsync();
            }

            return CreatedAtAction(nameof(Get), new { id = prop.Id }, new { prop.Id });
        }

        // POST api/properties/{id}/images
        // Owner uploads one or many images
        [HttpPost("{id}/images")]
        [Authorize(Roles = "Owner")]
        public async Task<ActionResult> UploadImages(int id, IFormFileCollection files)
        {
            var exists = await _db.Properties.AnyAsync(p => p.Id == id);
            if (!exists) return NotFound();
            if (files == null || files.Count == 0) return BadRequest("No files uploaded");

            var urls = new List<string>();
            foreach (var f in files) urls.Add(await _images.SavePropertyImageAsync(f));
            _db.PropertyImages.AddRange(urls.Select(u => new PropertyImage { PropertyId = id, ImageUrl = u }));
            await _db.SaveChangesAsync();
            return Ok(urls);
        }

        // PUT api/properties/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Owner")]
        public async Task<ActionResult> Update(int id, PropertyUpdateDto dto)
        {
            var prop = await _db.Properties.Include(p => p.PropertyAmenities).FirstOrDefaultAsync(p => p.Id == id);
            if (prop == null) return NotFound();

            prop.Title = dto.Title;
            prop.Description = dto.Description;
            prop.Address = dto.Address;
            prop.City = dto.City;
            prop.State = dto.State;
            prop.Country = dto.Country;
            prop.Latitude = dto.Latitude;
            prop.Longitude = dto.Longitude;
            prop.PricePerNight = dto.PricePerNight;
            prop.PropertyType = dto.PropertyType;
            prop.MaxGuests = dto.MaxGuests;
            prop.Bedrooms = dto.Bedrooms;
            prop.Bathrooms = dto.Bathrooms;
            prop.UpdatedAt = DateTime.UtcNow;

            _db.PropertyAmenities.RemoveRange(prop.PropertyAmenities);
            if (dto.AmenityIds.Any())
            {
                var toAdd = dto.AmenityIds.Select(aid => new PropertyAmenity { PropertyId = prop.Id, AmenityId = aid });
                _db.PropertyAmenities.AddRange(toAdd);
            }

            await _db.SaveChangesAsync();
            return NoContent();
        }

        // DELETE api/properties/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Owner")]
        public async Task<ActionResult> Delete(int id)
        {
            var prop = await _db.Properties.FindAsync(id);
            if (prop == null) return NotFound();
            _db.Properties.Remove(prop);
            await _db.SaveChangesAsync();
            return NoContent();
        }

        // GET api/properties/me
        [HttpGet("mine")]
        [Authorize(Roles = "Owner")]
        public async Task<ActionResult> Mine()
        {
            var sub = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(sub)) return Unauthorized();
            var ownerId = Guid.Parse(sub);

            var list = await _db.Properties.Where(p => p.OwnerId == ownerId)
                .Include(p => p.Images).ToListAsync();
            return Ok(list);
        }



        // GET api/properties/amenities
        [HttpGet("amenities")]
        public async Task<ActionResult<IEnumerable<Amenity>>> GetAmenities()
        {
            var amenities = await _db.Amenities.OrderBy(a => a.Id).ToListAsync();
            return Ok(amenities);
        }
    }
}
