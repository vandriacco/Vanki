namespace Vanki.API.Models
{
    public class DeckSummaryDto
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CardCount { get; set; }
    }
}
