using TextQuest.Domain.Interfaces;

namespace TextQuest.Domain.Objects
{
    public class Location : INameable
    {
        public string Name { get; set; }

        public IReadOnlyList<Character> Characters { get; set; }

        public bool HasUncompletedQuest => Characters.Any(x => x.HasUncompletedQuest);
    }
}
