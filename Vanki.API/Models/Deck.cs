using System.ComponentModel.DataAnnotations;

namespace Vanki.API.Models
{
    public class Deck
    {
        [Key]
        public Guid Id { get; init; } = Guid.NewGuid();
        [Required]
        public string Name { get; set; }
        [Required]
        public Guid UserId { get; set; }
        [Required]
        public User User { get; set; }
        public List<Card> Cards { get; set; } = new List<Card>();
        public DateTime CreatedDate { get; init; } = DateTime.UtcNow;

    }
}
