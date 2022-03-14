using TextQuest.Domain.Interfaces;

namespace TextQuest.Domain.Objects
{
    public class Location : INameable
    {
        public string Name { get; set; }
        public List<Character> Characters { get; set; } = new();
    }
}
