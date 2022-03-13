using TextQuest.Domain.Interfaces;

namespace TextQuest.Domain.Entities
{
    public class Character : INameable
    {
        public string Name { get; set; }
        public decimal MoneyBalance { get; set; }

        public IList<Quest> Quests { get; set; }
        public IList<Item> SelledItem { get; set; }
        public IReadOnlyList<Recipe> Crafts { get; set; }
    }
}
