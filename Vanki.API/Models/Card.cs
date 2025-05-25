using System.ComponentModel.DataAnnotations;

namespace Vanki.API.Models
{
    public class Card
    {
        [Key]
        public required Guid Id { get; init; }
        public string Front { get; set; }
        public string Back { get; set; }
    }
}
