using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using TextQuest.Application.Services;

namespace TextQuest.Application.UnitTests.Services
{
    [TestFixture]
    internal class WorldProviderTests
    {
        private static readonly WorldCreationParams creationParams = new()
        {
            QuestCount = 5..10,
            ItemTypeCount = 3..5,
            MonsterTypeCount = 2..4,

            CharactersInLocation = 2..10,
            QuestsForCharacter = 2..5,
            MaxItemCountForQuest = 3,

            MaxMonsterDropCount = 2,
            MaxMonsterDropType = 4,

            MaxMonsterTypeForQuest = 2,
            MaxMonsterCountForQuest = 3,
        };

        private Mock<INameGenerator> nameGeneratorMock;
        private Mock<IRandom> randomMock;
        private WorldProvider worldProvider;

        [SetUp]
        public void SetUp()
        {
            long nameCounter = 0;

            nameGeneratorMock = new();
            nameGeneratorMock.Setup(x => x.GetName(It.IsAny<NameGenerationParams>()))
                .Returns((NameGenerationParams _) => nameCounter++.ToString());

            randomMock = new();
            randomMock.Setup(x => x.Next(It.IsAny<System.Range>()))
                .Returns((System.Range range) => range.Start.Value);
            randomMock.Setup(x => x.NextElement(It.IsAny<List<It.IsAnyType>>()))
                .Returns((IEnumerable<object> list) => list.First());

            worldProvider = new(randomMock.Object, nameGeneratorMock.Object);
        }

        [Test]
        public void CreateNew_SetsWorld()
        {
            worldProvider.CreateNew(creationParams);

            worldProvider.World.Should().NotBeNull();
        }

        [Test]
        public void CreateNew_SetsAtLeastOneLocation()
        {
            worldProvider.CreateNew(creationParams);

            worldProvider.World.Locations.Should().NotBeNullOrEmpty();
        }

        [Test]
        public void CreateNew_SetsAtLeastOneCharacterInLocation()
        {
            worldProvider.CreateNew(creationParams);

            worldProvider.World.Locations.Should().AllSatisfy(
                x => x.Characters.Should().NotBeNullOrEmpty());
        }

        [Test]
        public void CreateNew_SetsAtLeastOneQuestToCharacter()
        {
            worldProvider.CreateNew(creationParams);

            worldProvider.World.Locations.Should().AllSatisfy(
                x => x.Characters.Should().AllSatisfy(
                    x => x.Quests.Should().NotBeNullOrEmpty()));
        }

        [Test]
        public void CreateNew_CreatesAtLeastOneQuestWithNoRequiredQuests()
        {
            worldProvider.CreateNew(creationParams);

            worldProvider.World.Locations
                .SelectMany(x => x.Characters)
                .SelectMany(x => x.Quests)
                .Should().Contain(
                x => !x.RequiredQuests.Any());
        }
    }
}
