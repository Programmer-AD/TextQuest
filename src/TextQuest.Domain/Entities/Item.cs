using TextQuest.Domain.Interfaces;

namespace TextQuest.Domain.Entities
{
    public class Item : INameable
    {
        public string Name { get; set; }
        public uint Count { get; set; }

        public decimal BuyPrice { get; set; }
        public decimal SellPrice => BuyPrice / DomainConstants.BuySellPriceCoefficient;
    }
}
