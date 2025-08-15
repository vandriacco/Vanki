using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;
using System.Text.RegularExpressions;
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
        private readonly IJwtService _jwt;
        private readonly PasswordHasher<User> _hasher = new();

        public AuthController(VankiDbContext db, IJwtService jwt)
        {
            _db = db;
            _jwt = jwt;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Username == request.Username && u.Email == request.Email);
            if (user == null)
            {
                return Unauthorized("Invalid username or password.");
            }

            var result = _hasher.VerifyHashedPassword(null, user.PasswordHash, request.Password);
            if (result != PasswordVerificationResult.Success)
                return Unauthorized("Invalid username or password.");

            var token = _jwt.GenerateToken(user.Id);
            return Ok(new { token });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Username))
            {
                return BadRequest("Username is required.");
            }

            if (string.IsNullOrWhiteSpace(request.Email))
            {
                return BadRequest("Email is required.");
            }

            var usernamePattern = @"^[a-zA-Z0-9_]{3,20}$";
            if (!Regex.IsMatch(request.Username, usernamePattern))
            {
                return BadRequest("Username must be 3-20 characters and contain only letters, numbers, or underscores.");
            }

            try
            {
                var email = new MailAddress(request.Email);
            }
            catch
            {
                return BadRequest("Invalid email format.");
            }

            if (await _db.Users.AnyAsync(u => u.Username == request.Username))
            {
                return BadRequest("Username already exists.");
            }

            if (await _db.Users.AnyAsync(u => u.Email == request.Email))
            {
                return BadRequest("Email already exists.");
            }

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