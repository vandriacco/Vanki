using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Vanki.API.Database;
using Vanki.API.Models;

namespace Vanki.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DecksController : ControllerBase
    {
        private readonly VankiDbContext _db;

        public DecksController(VankiDbContext context)
        {
            _db = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreateDeck([FromBody] Deck deck)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var username = User.FindFirst(ClaimTypes.Name)?.Value;

            if (userId == null)
            {
                return Unauthorized("User not authenticated.");
            }

            deck.UserId = Guid.Parse(userId);

            _db.Decks.Add(deck);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetDeck), new { deckId = deck.Id }, deck);
        }

        [HttpGet("{deckId}")]
        public async Task<IActionResult> GetDeck(Guid deckId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var deck = await _db.Decks
                .Include(x => x.Cards)
                .FirstOrDefaultAsync(x => x.Id == deckId && x.UserId.Equals(new Guid(userId)));

            if (deck == null)
            {
                return NotFound("Deck does not exist or you do not have access");
            }

            return Ok(deck);
        }

        [HttpGet]
        public async Task<IActionResult> GetDecks()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var decks = await _db.Decks
                .Where(x => x.UserId.Equals(new Guid(userId)))
                .ToListAsync();

            return Ok(decks);
        }
    }
}
