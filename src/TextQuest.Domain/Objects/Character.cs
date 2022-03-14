using TextQuest.Domain.Interfaces;

namespace TextQuest.Domain.Objects
{
    public class Character : INameable
    {
        public string Name { get; set; }
        public Location Location { get; set; }
        public List<Quest> Quests { get; set; } = new();
    }
}
