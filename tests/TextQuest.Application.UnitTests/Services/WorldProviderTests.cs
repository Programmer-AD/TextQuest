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
        private static readonly WorldCreationParams creationParams = new(5..10);

        private Mock<INameSetter> nameSetterMock;
        private Mock<IRandomGenerator> randomGeneratorMock;
        private WorldProvider worldProvider;

        [SetUp]
        public void SetUp()
        {
            nameSetterMock = new Mock<INameSetter>();
            randomGeneratorMock = new Mock<IRandomGenerator>();
            worldProvider = new WorldProvider(randomGeneratorMock.Object, nameSetterMock.Object);
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
        public void CreateNew_CreatesAtLeastOneQuestWithNoRequiredItemsOrQuests()
        {
            worldProvider.CreateNew(creationParams);

            worldProvider.World.Locations
                .SelectMany(x => x.Characters)
                .SelectMany(x => x.Quests)
                .Should().Contain(
                x => !(x.RequiredItems.Any() && x.RequiredQuests.Any()));
        }
    }
}
