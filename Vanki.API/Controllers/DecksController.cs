using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
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

        public DecksController(VankiDbContext db)
        {
            _db = db;
        }

        [HttpPost]
        public async Task<IActionResult> CreateDeck([FromBody] CreateDeckRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                return Unauthorized("User not authenticated.");
            }

            var deck = new Deck
            {
                UserId = Guid.Parse(userId),
                Name = request.Name
            };

            _db.Decks.Add(deck);
            await _db.SaveChangesAsync();

            var dto = new DeckDetailDto()
            {
                Id = deck.Id,
                Name = deck.Name,
                CreatedDate = deck.CreatedDate,
            };

            return CreatedAtAction(nameof(GetDeck), new { deckId = deck.Id }, dto);
        }

        [HttpGet("{deckId}")]
        public async Task<IActionResult> GetDeck(Guid deckId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(userId, out var userGuid))
            {
                return Unauthorized("User not authenticated.");
            }

            var deck = await _db.Decks
                .Where(d => d.UserId == userGuid && d.Id == deckId)
                .Select(d => new DeckDetailDto
                {
                    Id = d.Id,
                    Name = d.Name,
                    CreatedDate = d.CreatedDate,
                    Cards = d.Cards.Select(c => new CardDto
                    {
                        Id = c.Id,
                        Front = c.Front,
                        Back = c.Back,
                        ReviewDate = c.ReviewDate
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            if (deck == null)
            {
                return NotFound("Deck not found.");
            }

            return Ok(deck);
        }

        [HttpGet]
        public async Task<IActionResult> GetDecks()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(userId, out var userGuid))
            {
                return Unauthorized("User not authenticated.");
            }

            var decks = await _db.Decks
                .Where(d => d.UserId == userGuid)
                .Select(d => new DeckSummaryDto
                {
                    Id = d.Id,
                    Name = d.Name,
                    CreatedDate = d.CreatedDate,
                    CardCount = d.Cards.Count()
                })
                .ToListAsync();

            return Ok(decks);
        }

        [HttpPut("{deckId}")]
        public async Task<IActionResult> UpdateDeck(Guid deckId, [FromBody] UpdateDeckRequest updateDeckRequest)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(userId, out var userGuid))
            {
                return Unauthorized("User not authenticated.");
            }

            var deck = await _db.Decks
                .FirstOrDefaultAsync(x => x.Id == deckId && x.UserId == userGuid);

            if (deck == null)
            {
                return Forbid("Access denied.");
            }

            deck.Name = updateDeckRequest.Name;

            await _db.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{deckId}")]
        public async Task<IActionResult> DeleteDeck(Guid deckId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(userId, out var userGuid))
            {
                return Unauthorized("User not authenticated.");
            }

            var deck = await _db.Decks
                .Include(d => d.Cards)
                .FirstOrDefaultAsync(d => d.Id == deckId && d.UserId == userGuid);

            if (deck == null)
            {
                return Forbid("Access denied.");
            }

            _db.Remove(deck);

            await _db.SaveChangesAsync();

            return NoContent();
        }
    }
}
