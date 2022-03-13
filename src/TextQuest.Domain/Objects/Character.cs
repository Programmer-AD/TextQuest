using TextQuest.Domain.Interfaces;

namespace TextQuest.Domain.Objects
{
    public class Character : INameable
    {
        public string Name { get; set; }

        public IReadOnlyList<Quest> Quests { get; set; }

        public bool HasUncompletedQuest => Quests.Any(x => !x.Completed);
    }
}
