namespace TextQuest.Domain.Objects
{
    public class Player
    {
        public Location Location { get; set; }
        public IList<Quest> Quests { get; set; }
        public IList<Item> Items { get; set; }
    }
}
