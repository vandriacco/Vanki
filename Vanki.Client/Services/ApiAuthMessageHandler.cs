using Blazored.LocalStorage;
using System.Net.Http.Headers;

namespace Vanki.Client.Services
{
    public class ApiAuthMessageHandler : DelegatingHandler
    {
        private readonly ILocalStorageService _storage;
        public ApiAuthMessageHandler(ILocalStorageService storage) => _storage = storage;

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
        {
            var token = await _storage.GetItemAsStringAsync("authToken");
            if (!string.IsNullOrWhiteSpace(token))
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            return await base.SendAsync(request, ct);
        }
}
}