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
                return NotFound("Deck does not exist or you do not have access");
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
                return Unauthorized("Invalid user");
            }

            var card = await _db.Cards
                .Include(x => x.Deck)
                .FirstOrDefaultAsync(x => x.Id == cardId);

            if (card == null)
            {
                return NotFound("Card Not Found");
            }

            if (card.Deck == null)
            {
                return Forbid("You do not have access to this card.");
            }

            return Ok(card);
        }

        [HttpPut("{cardId}")]
        public async Task<IActionResult> UpdateCard(Guid cardId, [FromBody] UpdateRequest updateRequest)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(userId, out var userGuid))
            {
                return Unauthorized("Invalid user");
            }

            var card = await _db.Cards
                .Include(x => x.Deck)
                .FirstOrDefaultAsync(x => x.Id == cardId);

            if (card == null)
            {
                return NotFound("Card not found or access denied.");
            }

            if (card.Deck == null)
            {
                return Forbid("You do not have access to this card.");
            }

            card.Front = updateRequest.Front;
            card.Back = updateRequest.Back;

            await _db.SaveChangesAsync();

            return Ok(card);
        }

        [HttpDelete("{cardId}")]
        public async Task<IActionResult> DeleteCard(Guid cardId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(userId, out var userGuid))
            {
                return Unauthorized("Invalid user");
            }

            var card = await _db.Cards
                .Include(x => x.Deck)
                .FirstOrDefaultAsync(x => x.Id == cardId);

            if (card == null)
            {
                return NotFound("Card not found or access denied.");
            }

            if (card.Deck == null)
            {
                return Forbid("You do not have access to this card.");
            }

            _db.Remove(card);

            await _db.SaveChangesAsync();

            return NoContent();
        }
    }
}
