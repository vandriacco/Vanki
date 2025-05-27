using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Vanki.API.Database;
using Vanki.API.Models;
using Vanki.API.Services;

namespace Vanki.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly VankiDbContext _db;
        private readonly JwtService _jwt;
        private readonly PasswordHasher<User> _hasher = new();

        public AuthController(VankiDbContext db, JwtService jwt)
        {
            _db = db;
            _jwt = jwt;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Username == request.Username || u.Email == request.Email);
            if (user == null)
                return Unauthorized("Invalid username or password.");

            var result = _hasher.VerifyHashedPassword(null, user.PasswordHash, request.Password);
            if (result != PasswordVerificationResult.Success)
                return Unauthorized("Invalid username or password.");

            var token = _jwt.GenerateToken(user.Id);
            return Ok(new { token });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            if (await _db.Users.AnyAsync(u => u.Username == request.Username || u.Email == request.Email))
                return BadRequest("Username or email already exists.");
            var user = new User
            {
                Username = request.Username,
                Email = request.Email,
                PasswordHash = _hasher.HashPassword(null, request.Password)
            };
            _db.Users.Add(user);
            await _db.SaveChangesAsync();
            var token = _jwt.GenerateToken(user.Id);
            return CreatedAtAction(nameof(Login), new { token });
        }
    }
}