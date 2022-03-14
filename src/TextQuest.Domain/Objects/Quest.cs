namespace TextQuest.Domain.Objects
{
    public class Quest
    {
        public Character Giver { get; set; }
        public bool Completed { get; set; }
        public List<Quest> RequiredQuests { get; set; } = new();

        public List<Item> RequiredItems { get; set; } = new();
        public List<Item> ObtainedItems { get; set; } = new();
    }
}
