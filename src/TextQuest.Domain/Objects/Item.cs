using TextQuest.Domain.Interfaces;

namespace TextQuest.Domain.Objects
{
    public class Item : INameable
    {
        public string Name { get; set; }
    }
}
