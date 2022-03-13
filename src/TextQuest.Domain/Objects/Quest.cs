namespace TextQuest.Domain.Objects
{
    public class Quest
    {
        public Character Giver { get; set; }
        public bool Completed { get; set; }
        public IReadOnlyList<Quest> RequiredQuests { get; set; }

        public IReadOnlyList<Item> RequiredItems { get; set; }
        public IReadOnlyList<Item> ObtainedItems { get; set; }
    }
}
