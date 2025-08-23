using System.Net;
using System.Net.Http.Json;
using Vanki.API.Models;

namespace Vanki.Client.Services
{
    public class CardService
    {
        private readonly IHttpClientFactory _factory;
        public CardService(IHttpClientFactory factory) => _factory = factory;

        public async Task<CardDto?> GetCard(Guid deckId, Guid cardId)
        {
            var http = _factory.CreateClient("Api");
            return await http.GetFromJsonAsync<CardDto>($"decks/{deckId}/cards/{cardId}");
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

        public async Task<CardDto?> GetNextReview(Guid deckId)
        {
            var http = _factory.CreateClient("Api");

            var response = await http.GetAsync($"decks/{deckId}/cards/review/next", HttpCompletionOption.ResponseHeadersRead);

            if (response.StatusCode == HttpStatusCode.NoContent)
            {
                return null;
            }

            return await response.Content.ReadFromJsonAsync<CardDto>();
        }

        public async Task DeleteCard(Guid deckId, Guid cardId)
        {
            var http = _factory.CreateClient("Api");
            await http.DeleteAsync($"decks/{deckId}/cards/{cardId}");
        }
    }
}
