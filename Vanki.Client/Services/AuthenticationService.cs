using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using System.Net;
using System.Net.Http.Json;

namespace Vanki.Client.Services
{
    public interface IAuthenticationService
    {
        string? Token { get; }
        Task Initialize();
        Task<Result> Login(string identifier, string password);
        Task<Result> Register(string username, string email, string password);
        Task Logout();
    }

    public class AuthenticationService : IAuthenticationService
    {
        private readonly IHttpClientFactory _factory;
        private ILocalStorageService _storage;
        private NavigationManager _navigationManager;

        private record LoginRequest(string Identifier, string Password);
        private record AuthResponse(string Token);
        private record RegisterRequest(string Username, string Email, string Password);


        public string? Token { get; private set; }

        public AuthenticationService(
            IHttpClientFactory factory,
            ILocalStorageService localStorageService,
            NavigationManager navigationManager
        )
        {
            _factory = factory;
            _storage = localStorageService;
            _navigationManager = navigationManager;
        }

        public async Task Initialize()
        {
            Token = await _storage.GetItemAsync<string>("authToken");
        }

        public async Task<Result> Login(string identifier, string password)
        {
            var http = _factory.CreateClient("Api");

            var response = await http.PostAsJsonAsync("auth/login", new LoginRequest(identifier, password));
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                return new Result(false, "Unauthorized");
            }

            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                var message = string.IsNullOrWhiteSpace(body) ? $"Error {(int)response.StatusCode}" : body;
                return new Result(false, message);
            }

            var data = await response.Content.ReadFromJsonAsync<AuthResponse>();
            if (string.IsNullOrWhiteSpace(data?.Token))
            {
                return new Result(false, "No token in response.");
            }

            Token = data.Token;
            await _storage.SetItemAsStringAsync("authToken", Token);
            return new Result(true, null);
        }

        public async Task<Result> Register(string username, string email, string password)
        {
            var http = _factory.CreateClient("Api");

            var response = await http.PostAsJsonAsync("auth/register", new RegisterRequest(username, email, password));
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                return new Result(false, "Unauthorized");
            }

            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                var message = string.IsNullOrWhiteSpace(body) ? $"Error {(int)response.StatusCode}" : body;
                return new Result(false, message);
            }

            var data = await response.Content.ReadFromJsonAsync<AuthResponse>();
            if (string.IsNullOrWhiteSpace(data?.Token))
            {
                return new Result(false, "No token in response.");
            }

            Token = data.Token;
            await _storage.SetItemAsStringAsync("authToken", Token);
            return new Result(true, null);
        }

        public async Task Logout()
        {
            Token = null;
            await _storage.RemoveItemAsync("authToken");
            _navigationManager.NavigateTo("login", true);
        }

    }
}
