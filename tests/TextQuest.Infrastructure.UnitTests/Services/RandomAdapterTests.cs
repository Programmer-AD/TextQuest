using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace TextQuest.Infrastructure.UnitTests.Services
{
    [TestFixture]
    public class RandomAdapterTests
    {
        private static readonly List<int> list = Enumerable.Range(0, 30).ToList();

        private RandomAdapter randomAdapter;

        [SetUp]
        public void SetUp()
        {
            randomAdapter = new();
        }

        [Test]
        public void Mix_KeepsAllElements()
        {
            var listCopy = list.ToList();

            randomAdapter.Mix(listCopy);

            listCopy.Should().BeEquivalentTo(list);
        }

        [Test, Retry(3)]
        public void Mix_ChangeElementOrders()
        {
            var listCopy = list.ToList();

            randomAdapter.Mix(listCopy);

            listCopy.Should().NotEqual(list);
        }

        [TestCase(1)]
        [TestCase(5)]
        public void Next_IncludesRangeBorderValues(int border)
        {
            var value = randomAdapter.Next(border..border);

            value.Should().Be(border);
        }
    }
}
