using Vanki.API.Models;

namespace Vanki.API.Services
{
    public interface ISrsService
    {
        void ReviewCard(Card card, int quality);
    }
}