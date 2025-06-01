using Vanki.API.Database;
using Vanki.API.Models;

namespace Vanki.API.Services
{
    public class SrsService : ISrsService
    {
        public void ReviewCard(Card card, int quality)
        {
            card.Repetitions = quality >= 3 ? card.Repetitions + 1 : 0;

            if (card.Repetitions == 1)
            {
                card.Interval = 1;
            }
            else if (card.Repetitions == 2)
            {
                card.Interval = 6;
            }
            else
            {
                card.Interval = card.Interval * card.EfficiencyScore;
            }


            card.EfficiencyScore = card.EfficiencyScore - 0.8 + 0.28 * quality - 0.02 * quality * quality;
            card.EfficiencyScore = Math.Max(card.EfficiencyScore, 1.3);
            card.ReviewDate = DateTime.UtcNow.AddDays(card.Interval);
        }
    }
}
