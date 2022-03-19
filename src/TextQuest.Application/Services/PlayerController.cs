using TextQuest.Application.Exceptions;
using TextQuest.Application.Interfaces;

namespace TextQuest.Application.Services
{
    internal class PlayerController : IPlayerController
    {
        private readonly Player player;

        public PlayerController()
        {
            player = new();
        }

        public Location CurrentLocation => player.Location;

        public IEnumerable<Quest> Quests => player.Quests;

        public IEnumerable<Counted<Item>> Items => player.Items;

        public void ExchangeQuestItems(Character reciever)
        {
            CheckCanExchangeItems(reciever);
            var quest = GetExchangeQuest(reciever);

            foreach (var item in quest.RequiredItems)
            {
                player.Items.Remove(item);
            }
            foreach (var item in quest.ObtainedItems)
            {
                player.Items.Add(item);
            }

            quest.Completed = true;
            player.Quests.Remove(quest);
        }

        public void MoveTo(Location newLocation)
        {
            player.Location = newLocation;
        }

        public void PickQuest(Quest quest)
        {
            CheckCanBeAdded(quest);

            player.Quests.Add(quest);
        }

        public bool HasQuest(Quest quest)
        {
            return player.Quests.Contains(quest);
        }

        private void CheckCanExchangeItems(Character reciever)
        {
            if (reciever.Location != player.Location)
            {
                throw new ItemExchangeException("Cant exchange with other location");
            }
        }

        private Quest GetExchangeQuest(Character itemReciever)
        {
            var quest = Quests.FirstOrDefault(
                x => x.Giver == itemReciever
                && x.RequiredItems.All(
                    item => player.Items.Contains(item)));

            if (quest == null)
            {
                throw new ItemExchangeException("No quests for this character or you dont have all required items");
            }

            return quest;
        }


        private void CheckCanBeAdded(Quest quest)
        {
            foreach (var (checker, message) in GetQuestCheckers())
            {
                if (checker(quest))
                {
                    throw new QuestAddingException(message);
                }
            }
        }

        private IEnumerable<(Predicate<Quest> checker, string message)> GetQuestCheckers()
        {
            yield return
                (quest => quest.Completed, "Quest already completed!");

            yield return
                (quest => quest.Giver.Location != player.Location, "Cant add quest from other location!");

            yield return
                (quest => quest.RequiredQuests.Any(x => !x.Completed), "Not all required quests completed!");

            yield return
                (HasQuest, "Quest already added!");
        }
    }
}
