namespace TextQuest.Domain.Objects
{
    public class Location : INameable
    {
        public string Name { get; set; }
        public List<Character> Characters { get; } = new();
        public CountedCollection<Monster> Monsters { get; } = new();

        public int QuestCount => Characters.Sum(x => x.QuestCount);
        public int CompletedQuestCount => Characters.Sum(x => x.CompletedQuestCount);
    }
}
