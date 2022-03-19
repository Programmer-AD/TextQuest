namespace TextQuest.Domain.Objects
{
    public class Quest
    {
        public Character Giver { get; set; }
        public bool Completed { get; set; }
        public List<Quest> RequiredQuests { get; set; } = new();

        public CountedCollection<Item> RequiredItems { get; set; } = new();
        public CountedCollection<Item> ObtainedItems { get; set; } = new();
    }
}
