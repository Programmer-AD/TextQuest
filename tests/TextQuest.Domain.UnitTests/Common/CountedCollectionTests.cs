using System;
using FluentAssertions;
using NUnit.Framework;
using TextQuest.Domain.Common;

namespace TextQuest.Domain.UnitTests.Common
{
    [TestFixture]
    public class CountedCollectionTests
    {
        private static readonly Guid key1 = Guid.NewGuid();
        private static readonly Guid key2 = Guid.NewGuid();
        private static readonly Guid keyForEmpty = Guid.NewGuid();

        private static readonly Counted<Guid> item1 = new(key1, 10);
        private static readonly Counted<Guid> item2 = new(key2, 15);
        private static readonly Counted<Guid> emptyItem = new(keyForEmpty, 0);


        private CountedCollection<Guid> collection;

        [SetUp]
        public void SetUp()
        {
            collection = new();
        }

        [Test]
        public void Add_WhenCountIsZero_DoNothing()
        {
            collection.Add(emptyItem);

            collection.Should().BeEmpty();
        }

        [Test]
        public void Add_WhenCountGreaterThenZero_AddItemEquivalent()
        {
            collection.Add(item1);

            collection.Should().ContainEquivalentOf(item1);
        }

        [Test]
        public void Add_WhenAddingTwoDifferent_AddEquivalentsOfBoth()
        {
            collection.Add(item1);

            collection.Add(item2);

            collection.Should().ContainEquivalentOf(item1)
                .And.ContainEquivalentOf(item2);
        }

        [Test]
        public void Add_WhenAddingSame_SumCounts()
        {
            collection.Add(item1);
            var same = new Counted<Guid>(item1.Value, 5);
            var expected = new Counted<Guid>(item1.Value, item1.Count + same.Count);

            collection.Add(same);

            collection.Should().HaveCount(1).And.ContainEquivalentOf(expected);
        }

        [Test]
        public void Add_WhenAddingSame_DoesntChangeFirstAddedCount()
        {
            collection.Add(item1);
            var initCount = item1.Count;
            var same = new Counted<Guid>(item1.Value, 5);

            collection.Add(same);

            item1.Count.Should().Be(initCount);
        }

        [Test]
        public void Clear_RemoveAllItems()
        {
            collection.Add(item1);
            collection.Add(item2);

            collection.Clear();

            collection.Should().BeEmpty();
        }

        [Test]
        public void Contains_WhenNoSuchItem_ReturnsFalse()
        {
            collection.Add(item1);

            var result = collection.Contains(item2);

            result.Should().BeFalse();
        }

        [Test]
        public void Contains_WhenExistingCountIsLessThenInArgument_ReturnsFalse()
        {
            collection.Add(item1);
            var argumentItem = new Counted<Guid>(item1.Value, item1.Count + 1);

            var result = collection.Contains(argumentItem);

            result.Should().BeFalse();
        }

        [Test]
        public void Contains_WhenExistingCountIsEqualsToArguments_ReturnsTrue()
        {
            collection.Add(item1);
            var argumentItem = new Counted<Guid>(item1.Value, item1.Count);

            var result = collection.Contains(argumentItem);

            result.Should().BeTrue();
        }

        [Test]
        public void Contains_WhenExistingCountIsGreaterThenInArgument_ReturnsTrue()
        {
            collection.Add(item1);
            var argumentItem = new Counted<Guid>(item1.Value, item1.Count - 1);

            var result = collection.Contains(argumentItem);

            result.Should().BeTrue();
        }

        [Test]
        public void Remove_WhenCountIsZero_DoNothingAndReturnFalse()
        {
            collection.Add(item1);
            var argumentItem = new Counted<Guid>(item1.Value, 0);

            var result = collection.Remove(argumentItem);

            collection.Should().ContainEquivalentOf(item1);
            result.Should().BeFalse();
        }

        [Test]
        public void Remove_WhenNoSuchElement_DoNothingAndReturnFalse()
        {
            collection.Add(item1);

            var result = collection.Remove(item2);

            collection.Should().ContainEquivalentOf(item1);
            result.Should().BeFalse();
        }

        [Test]
        public void Remove_WhenExistingCountIsLessThenInArgument_DoNothingAndReturnFalse()
        {
            collection.Add(item1);
            var argumentItem = new Counted<Guid>(item1.Value, item1.Count + 1);

            var result = collection.Remove(argumentItem);

            collection.Should().ContainEquivalentOf(item1);
            result.Should().BeFalse();
        }

        [Test]
        public void Remove_WhenExistingCountIsGreaterThenInArgument_SubtractCountAndReturnTrue()
        {
            collection.Add(item1);
            var argumentItem = new Counted<Guid>(item1.Value, item1.Count - 1);
            var expected = new Counted<Guid>(item1.Value, item1.Count - argumentItem.Count);

            var result = collection.Remove(argumentItem);

            collection.Should().ContainEquivalentOf(expected);
            result.Should().BeTrue();
        }

        [Test]
        public void Remove_WhenExistingCountIsEqualToArguments_RemoveItemAndReturnTrue()
        {
            collection.Add(item1);
            var argumentItem = new Counted<Guid>(item1.Value, item1.Count);

            var result = collection.Remove(argumentItem);

            collection.Should().NotContain(x => x.Value == item1.Value);
            result.Should().BeTrue();
        }
    }
}
