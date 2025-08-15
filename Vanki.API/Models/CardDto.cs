namespace Vanki.API.Models
{
    public class CardDto
    {
        public Guid Id { get; set; }
        public string Front { get; set; }
        public string Back { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ReviewDate { get; set; }
    }
}
