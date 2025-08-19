using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Vanki.API.Models
{
    public class Card
    {
        [Key]
        [Required]
        public Guid Id { get; init; } = Guid.NewGuid();
        [Required]
        public Guid DeckId { get; set; }
        [JsonIgnore]
        public Deck Deck { get; set; }
        [Required]
        public string Front { get; set; }
        [Required]
        public string Back { get; set; }
        public double Interval { get; set; } = 0;
        public double EfficiencyScore { get; set; } = 2.5;
        public int Repetitions { get; set; } = 0;
        public DateTime CreatedDate { get; init; } = DateTime.UtcNow;
        public DateTime? ReviewDate { get; set; }
    }
}
