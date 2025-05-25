using System.ComponentModel.DataAnnotations;

namespace Vanki.API.Models
{
    public class Deck
    {
        [Key]
        public Guid Id { get; init; }
        public string? Name { get; set; }
        public List<Card>? Cards { get; set; } = new List<Card>();
    }
}
