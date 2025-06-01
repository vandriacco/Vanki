using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Vanki.API.Models
{
    public class User
    {
        [Key]
        public Guid Id { get; init; } = Guid.NewGuid();
        [Required]
        [MaxLength(100)]
        public string Username { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string PasswordHash { get; set; }
        public List<Deck> Decks { get; set; }
        public DateTime CreatedDate { get; init; } = DateTime.UtcNow;

    }
}
