namespace Vanki.API.Models
{
    public class LoginRequest
    {
        public string Identifier { get; set; }
        public string Password { get; set; } = string.Empty;
    }
}
