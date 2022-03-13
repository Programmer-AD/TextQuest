using TextQuest.Domain.Interfaces;

namespace TextQuest.Domain.Entities
{
    public class Location : INameable
    {
        public string Name { get; set; }

        public IReadOnlyList<Character> Characters { get; set; }
    }
}
