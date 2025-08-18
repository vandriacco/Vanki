using System.Net.Http.Json;
using Vanki.API.Models;

namespace Vanki.Client.Services
{
    public class CardService
    {
        private readonly IHttpClientFactory _factory;
        public CardService(IHttpClientFactory factory) => _factory = factory;

        public async Task<List<CardDto>?> GetCards(Guid deckId)
        {
            var http = _factory.CreateClient("Api");
            return await http.GetFromJsonAsync<List<CardDto>>($"decks/{deckId}/cards");
        }

        public async Task CreateCard(Guid deckId, string front, string back)
        {
            var http = _factory.CreateClient("Api");
            await http.PostAsJsonAsync($"decks/{deckId}/cards", new CreateCardRequest{ Front = front, Back = back });
        }

        public async Task UpdateCard(Guid deckId, Guid cardId, string front, string back)
        {
            var http = _factory.CreateClient("Api");
            await http.PutAsJsonAsync($"decks/{deckId}/cards/{cardId}", new UpdateCardRequest { Front = front, Back = back });
        }

        public async Task ReviewCard(Guid deckId, Guid cardId, int quality)
        {
            var http = _factory.CreateClient("Api");
            await http.PutAsJsonAsync($"decks/{deckId}/cards/review/{cardId}", new ReviewRequest { Quality = quality});
        }

        public async Task DeleteDeck(Guid deckId)
        {
            var http = _factory.CreateClient("Api");
            await http.DeleteAsync($"decks/{deckId}");
        }
    }
}
