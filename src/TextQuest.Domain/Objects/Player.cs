namespace TextQuest.Domain.Objects
{
    public class Player
    {
        public Location Location { get; set; }
        public List<Quest> Quests { get; set; } = new();
        public List<Item> Items { get; set; } = new();
    }
}
