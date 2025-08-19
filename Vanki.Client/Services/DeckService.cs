using System.Net.Http.Json;
using System.Xml.Linq;
using Vanki.API.Models;

namespace Vanki.Client.Services
{
    public class DeckService
    {
        private readonly IHttpClientFactory _factory;
        public DeckService(IHttpClientFactory factory) => _factory = factory;

        public async Task<List<DeckSummaryDto>?> GetDecks()
        {
            var http = _factory.CreateClient("Api");
            return await http.GetFromJsonAsync<List<DeckSummaryDto>>("decks");
        }

        public async Task<DeckDetailDto?> GetDeck(Guid deckId)
        {
            var http = _factory.CreateClient("Api");
            return await http.GetFromJsonAsync<DeckDetailDto>($"decks/{deckId}");
        }

        public async Task CreateDeck(string name)
        {
            var http = _factory.CreateClient("Api");
            await http.PostAsJsonAsync("decks", new CreateDeckRequest { Name = name });
        }

        public async Task UpdateDeck(Guid deckId, string name)
        {
            var http = _factory.CreateClient("Api");
            await http.PutAsJsonAsync($"decks/{deckId}", new UpdateDeckRequest { Name = name });
        }

        public async Task DeleteDeck(Guid deckId)
        {
            var http = _factory.CreateClient("Api");
            await http.DeleteAsync($"decks/{deckId}");
        }
    }
}
