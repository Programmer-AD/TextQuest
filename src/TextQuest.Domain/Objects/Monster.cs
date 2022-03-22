namespace TextQuest.Domain.Objects
{
    public class Monster : INameable
    {
        public string Name { get; set; }
        public CountedCollection<Item> DroppedItems { get; set; } = new();
    }
}
