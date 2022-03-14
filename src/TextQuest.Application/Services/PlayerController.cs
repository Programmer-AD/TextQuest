using TextQuest.Application.Exceptions;
using TextQuest.Application.Interfaces;

namespace TextQuest.Application.Services
{
    internal class PlayerController : IPlayerController
    {
        private readonly Player player;

        public PlayerController()
        {
            player = new Player
            {
                Items = new List<Item>(),
                Quests = new List<Quest>()
            };
        }

        public Location CurrentLocation => player.Location;

        public IReadOnlyList<Quest> Quests => (IReadOnlyList<Quest>)player.Quests;

        public IReadOnlyList<Item> Items => (IReadOnlyList<Item>)player.Items;

        public void ExchangeQuestItems(Character reciever)
        {
            CheckCanExchangeItems(reciever);
            var quest = GetExchangeQuest(reciever);

            var playerItemSet = player.Items.ToHashSet();
            playerItemSet.ExceptWith(quest.RequiredItems);
            playerItemSet.UnionWith(quest.ObtainedItems);
            player.Items = playerItemSet.ToList();

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

        private void CheckCanExchangeItems(Character reciever)
        {
            if (reciever.Location != player.Location)
            {
                throw new ItemExchangeException("Cant exchange with other location");
            }
        }

        private Quest GetExchangeQuest(Character itemReciever)
        {
            var playerItemSet = player.Items.ToHashSet();
            var quest = Quests.Where(
                x => x.Giver == itemReciever
                && playerItemSet.IsSupersetOf(x.RequiredItems))
                .FirstOrDefault();

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
                (quest => player.Quests.Contains(quest), "Quest already added!");
        }
    }
}
