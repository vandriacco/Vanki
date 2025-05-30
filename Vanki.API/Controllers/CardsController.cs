using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Vanki.API.Database;
using Vanki.API.Migrations;
using Vanki.API.Models;

namespace Vanki.API.Controllers
{
    [ApiController]
    [Route("api/{deckId}/[controller]")]
    [Authorize]
    public class CardsController : ControllerBase
    {
        private readonly VankiDbContext _db;

        public CardsController(VankiDbContext context)
        {
            _db = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreateCard(Guid deckId, [FromBody] Card card)
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

            card.DeckId = deckId;

            _db.Cards.Add(card);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCard), new { cardId = card.Id }, card);
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
