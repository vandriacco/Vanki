namespace Vanki.API.Models
{
    public class DeckDetailDto
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public DateTime CreatedDate { get; set; }
        public List<CardDto>? Cards { get; set; }
    }
}
