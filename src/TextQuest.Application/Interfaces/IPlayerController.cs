namespace TextQuest.Application.Interfaces
{
    public interface IPlayerController
    {
        public Location CurrentLocation { get; }
        public IReadOnlyList<Quest> Quests { get; }
        public IEnumerable<Counted<Item>> Items { get; }

        void MoveTo(Location newLocation);
        void ExchangeQuestItems(Character reciever);
        void PickQuest(Quest quest);
    }
}
