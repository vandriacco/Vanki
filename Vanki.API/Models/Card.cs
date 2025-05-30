using System.ComponentModel.DataAnnotations;

namespace Vanki.API.Models
{
    public class Card
    {
        [Key]
        [Required]
        public Guid Id { get; init; } = Guid.NewGuid();
        [Required]
        public Guid DeckId { get; set; }
        public Deck Deck { get; set; }
        public string Front { get; set; }
        public string Back { get; set; }
    }
}
