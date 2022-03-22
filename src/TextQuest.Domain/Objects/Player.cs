namespace TextQuest.Domain.Objects
{
    public class Player
    {
        public Location Location { get; set; }
        public HashSet<Quest> Quests { get; } = new();
        public CountedCollection<Item> Items { get; } = new();
    }
}
