using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RentAPlace.Api.Data;
using RentAPlace.Api.Dtos;
using RentAPlace.Api.Models;
using RentAPlace.Api.Services;
using BCrypt.Net;

namespace RentAPlace.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IJwtService _jwt;
        public AuthController(AppDbContext db, IJwtService jwt)
        {
            _db = db; _jwt = jwt;
        }

        // POST api/auth/register
        [HttpPost("register")]
        public async Task<ActionResult<AuthResponse>> Register(RegisterDto dto)
        {
            if (await _db.Users.AnyAsync(u => u.Email == dto.Email))
                return BadRequest("Email already registered");

            var user = new User
            {
                Id = Guid.NewGuid(),
                FullName = dto.FullName,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = dto.Role,
                Phone = dto.Phone
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            var token = _jwt.Generate(user);
            return Ok(new AuthResponse { Token = token, FullName = user.FullName, Role = user.Role, Email = user.Email });
        }

        // POST api/auth/login
        [HttpPost("login")]
        public async Task<ActionResult<AuthResponse>> Login(LoginDto dto)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                return Unauthorized("Invalid email or password");

            var token = _jwt.Generate(user);
            return Ok(new AuthResponse { Token = token, FullName = user.FullName, Role = user.Role, Email = user.Email });
        }
    }
}
