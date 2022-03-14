using TextQuest.Domain.Interfaces;

namespace TextQuest.Domain.Objects
{
    public class World : INameable
    {
        public string Name { get; set; }
        public List<Location> Locations { get; set; } = new();
    }
}
