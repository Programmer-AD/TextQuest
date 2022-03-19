namespace TextQuest.Domain.Objects
{
    public class World : INameable
    {
        public string Name { get; set; }
        public List<Location> Locations { get; set; } = new();

        public int QuestCount => Locations.Sum(x => x.QuestCount);
        public int CompletedQuestCount => Locations.Sum(x => x.CompletedQuestCount);
    }
}
