using TextQuest.Domain.Interfaces;

namespace TextQuest.Domain.Objects
{
    public class Character : INameable
    {
        public string Name { get; set; }
        public Location Location { get; set; }
        public List<Quest> Quests { get; set; } = new();

        public int QuestCount => Quests.Count;
        public int CompletedQuestCount => Quests.Count(x => x.Completed);

        public IEnumerable<Quest> AvailableQuests => 
            Quests.Where(x => !x.Completed && x.RequiredQuests.All(x => x.Completed));

        public Quest RecomendedQuest =>
            Quests.FirstOrDefault(x => !x.Completed)
            .RequiredQuests.FirstOrDefault(x => !x.Completed);
    }
}
