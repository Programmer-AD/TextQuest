using TextQuest.Domain.Interfaces;

namespace TextQuest.Domain.Objects
{
    public class World : INameable
    {
        public string Name { get; set; }
        public IReadOnlyList<Location> Locations { get; set; }

        public bool HasUncompletedQuest => Locations.Any(x => x.HasUncompletedQuest);
    }
}
