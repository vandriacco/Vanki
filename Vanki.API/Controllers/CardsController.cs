using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Vanki.API.Database;
using Vanki.API.Migrations;
using Vanki.API.Models;
using Vanki.API.Services;

namespace Vanki.API.Controllers
{
    [ApiController]
    [Route("api/{deckId}/[controller]")]
    [Authorize]
    public class CardsController : ControllerBase
    {
        private readonly VankiDbContext _db;
        private readonly ISrsService _srsService;

        public CardsController(VankiDbContext db, ISrsService srsService)
        {
            _db = db;
            _srsService = srsService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateCard(Guid deckId, [FromBody] CreateCardRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                return Unauthorized("User not authenticated.");
            }

            var deck = await _db.Decks
                .Include(x => x.Cards)
                .FirstOrDefaultAsync(x => x.Id == deckId && x.UserId.Equals(new Guid(userId)));

            if (deck == null)
            {
                return Forbid("Access denied.");
            }

            var card = new Card
            {
                DeckId = deckId,
                Front = request.Front,
                Back = request.Back
            };

            _db.Cards.Add(card);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCard), new { deckId = card.DeckId, cardId = card.Id }, card);
        }

        [HttpGet("{cardId}")]
        public async Task<IActionResult> GetCard(Guid cardId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(userId, out var userGuid))
            {
                return Unauthorized("User not authenticated.");
            }

            var card = await _db.Cards
                .Include(x => x.Deck)
                .FirstOrDefaultAsync(x => x.Id == cardId && x.Deck.UserId == userGuid);

            if (card == null)
            {
                return NotFound("Card not found.");
            }

            return Ok(card);
        }

        [HttpPut("{cardId}")]
        public async Task<IActionResult> UpdateCard(Guid cardId, [FromBody] UpdateCardRequest updateRequest)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(userId, out var userGuid))
            {
                return Unauthorized("User not authenticated.");
            }

            var card = await _db.Cards
                .Include(x => x.Deck)
                .FirstOrDefaultAsync(x => x.Id == cardId && x.Deck.UserId == userGuid);

            if (card == null)
            {
                return NotFound("Card not found.");
            }

            card.Front = updateRequest.Front;
            card.Back = updateRequest.Back;

            await _db.SaveChangesAsync();

            return NoContent();
        }

        [HttpPut("review/{cardId}")]
        public async Task<IActionResult> ReviewCard(Guid cardId, [FromBody] ReviewRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(userId, out var userGuid))
            {
                return Unauthorized("User not authenticated.");
            }

            var card = await _db.Cards
                .Include(x => x.Deck)
                .FirstOrDefaultAsync(x => x.Id == cardId && x.Deck.UserId == userGuid);

            if (card == null)
            {
                return NotFound("Card not found.");
            }

            if (request.Quality is < 0 or > 5)
            {
                return BadRequest("Quality must be between 0 and 5.");
            }

            _srsService.ReviewCard(card, request.Quality);
            await _db.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{cardId}")]
        public async Task<IActionResult> DeleteCard(Guid cardId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(userId, out var userGuid))
            {
                return Unauthorized("User not authenticated.");
            }

            var card = await _db.Cards
                .Include(x => x.Deck)
                .FirstOrDefaultAsync(x => x.Id == cardId && x.Deck.UserId == userGuid);

            if (card == null)
            {
                return NotFound("Card not found.");
            }

            _db.Remove(card);

            await _db.SaveChangesAsync();

            return NoContent();
        }
    }
}
