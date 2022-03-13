using NUnit.Framework;
using FluentAssertions;
using TextQuest.Application.Services;

namespace TextQuest.Application.UnitTests.Services
{
    [TestFixture]
    internal class PlayerControllerTests
    {
        private static readonly Location location = new();
        private static readonly Location otherLocation = new();
        private static readonly Character questGiver = new()
        {
            Location = location,
            Quests = new[] { uncompletedQuest, uncompletedQuest2, completedQuest },
        };
        private static readonly Character otherQuestGiver = new()
        {
            Location = location,
            Quests = new[] { uncompletedQuestFromOtherGiver }
        };
        private static readonly Item item1 = new();
        private static readonly Item item2 = new();
        private static readonly Quest uncompletedQuest = new()
        {
            Completed = false,
            Giver = questGiver,
            RequiredQuests = Array.Empty<Quest>(),
            ObtainedItems = new[] { item1 },
            RequiredItems = Array.Empty<Item>(),
        };
        private static readonly Quest uncompletedQuest2 = new()
        {
            Completed = false,
            Giver = questGiver,
            RequiredQuests = Array.Empty<Quest>(),
            ObtainedItems = new[] { item2 },
            RequiredItems = new[] { item1 },
        };
        private static readonly Quest uncompletedQuestFromOtherGiver = new()
        {
            Completed = false,
            Giver = otherQuestGiver,
            RequiredQuests = Array.Empty<Quest>(),
            ObtainedItems = Array.Empty<Item>(),
            RequiredItems = Array.Empty<Item>(),
        };
        private static readonly Quest completedQuest = new()
        {
            Completed = true,
            Giver = questGiver,
            RequiredQuests = Array.Empty<Quest>(),
        };


        private PlayerController playerController;

        [SetUp]
        public void SetUp()
        {
            uncompletedQuest.Completed =
                uncompletedQuest2.Completed =
                uncompletedQuestFromOtherGiver.Completed = false;

            completedQuest.Completed = true;

            playerController = new PlayerController();
        }

        [Test]
        public void ExchangeQuestItems_ObtainsNewItems()
        {
            playerController.MoveTo(location);
            playerController.PickQuest(uncompletedQuest);

            playerController.ExchangeQuestItems(questGiver);

            playerController.Items.Should().Contain(item1);
        }

        [Test]
        public void ExchangeQuestItems_RemovesGivenItems()
        {
            playerController.MoveTo(location);
            playerController.PickQuest(uncompletedQuest);
            playerController.ExchangeQuestItems(questGiver);
            playerController.PickQuest(uncompletedQuest2);

            playerController.ExchangeQuestItems(questGiver);

            playerController.Items.Should().NotContain(item1).And.Contain(item2);
        }

        [Test]
        public void ExchangeQuestItems_MarksQuestCompleted()
        {
            playerController.MoveTo(location);
            playerController.PickQuest(uncompletedQuest);

            playerController.ExchangeQuestItems(questGiver);

            uncompletedQuest.Completed.Should().BeTrue();
        }

        [Test]
        public void ExchangeQuestItems_RemovesCompletedQuestFromPlayer()
        {
            playerController.MoveTo(location);
            playerController.PickQuest(uncompletedQuest);
            playerController.PickQuest(uncompletedQuestFromOtherGiver);

            playerController.ExchangeQuestItems(questGiver);

            playerController.Quests.Should().NotContain(uncompletedQuest)
                .And.Contain(uncompletedQuestFromOtherGiver);
        }

        [Test]
        public void ExchangeQuestItems_WhenNoQuestsFromItemReciever_ThrowsItemExchangeException()
        {
            playerController.MoveTo(location);
            playerController.PickQuest(uncompletedQuestFromOtherGiver);

            playerController.Invoking(x => x.ExchangeQuestItems(questGiver))
                .Should().Throw<ItemExchangeException>();
        }

        [Test]
        public void ExchangeQuestItems_WhenNoItemsForRecieverQuests_ThrowsItemExchangeException()
        {
            playerController.MoveTo(location);
            playerController.PickQuest(uncompletedQuest2);

            playerController.Invoking(x => x.ExchangeQuestItems(questGiver))
                .Should().Throw<ItemExchangeException>();
        }

        [Test]
        public void ExchangeQuestItems_WhenExchangeWithOtherLocation_ThrowsItemExchangeException()
        {
            playerController.MoveTo(location);
            playerController.PickQuest(uncompletedQuest);
            playerController.MoveTo(otherLocation);


            playerController.Invoking(x => x.ExchangeQuestItems(questGiver))
                .Should().Throw<ItemExchangeException>();
        }

        [Test]
        public void MoveTo_SetsNewLocation()
        {
            var newLocation = new Location();

            playerController.MoveTo(newLocation);

            playerController.CurrentLocation.Should().Be(newLocation);
        }

        [Test]
        public void PickQuest_WhenNoQuests_AddsQuest()
        {
            playerController.MoveTo(location);

            playerController.PickQuest(uncompletedQuest);

            playerController.Quests.Should().Contain(uncompletedQuest);
        }

        [Test]
        public void PickQuest_WhenHasOtherQuest_AddsQuest()
        {
            playerController.MoveTo(location);
            playerController.PickQuest(uncompletedQuest);

            playerController.PickQuest(uncompletedQuest2);

            playerController.Quests.Should().Contain(uncompletedQuest)
                .And.Contain(uncompletedQuest2);
        }

        [Test]
        public void PickQuest_WhenAddingSameQuestTwice_ThrowsQuestAddingException()
        {
            playerController.MoveTo(location);
            playerController.PickQuest(uncompletedQuest);

            playerController.Invoking(x => x.PickQuest(uncompletedQuest))
                .Should().Throw<QuestAddingException>();
        }

        [Test]
        public void PickQuest_WhenAddingCompletedQuest_ThrowsQuestAddingException()
        {
            playerController.MoveTo(location);

            playerController.Invoking(x => x.PickQuest(completedQuest))
                .Should().Throw<QuestAddingException>();
        }

        [Test]
        public void PickQuest_WhenAddingQuestFromOtherLocationCharacter_ThrowsQuestAddingException()
        {
            playerController.MoveTo(otherLocation);

            playerController.Invoking(x => x.PickQuest(uncompletedQuest))
                .Should().Throw<QuestAddingException>();
        }

        [Test]
        public void PickQuest_WhenNotAllRequiredQuestsCompleted_ThrowsQuestAddingException()
        {
            playerController.MoveTo(location);
            var questWithRequirement = new Quest()
            {
                Completed = false,
                Giver = questGiver,
                RequiredQuests = new[] { uncompletedQuest }
            };

            playerController.Invoking(x => x.PickQuest(questWithRequirement))
                .Should().Throw<QuestAddingException>();
        }
    }
}
