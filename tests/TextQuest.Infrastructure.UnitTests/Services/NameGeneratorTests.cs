using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using TextQuest.Application.Models;

namespace TextQuest.Infrastructure.UnitTests.Services
{
    [TestFixture]
    internal class NameGeneratorTests
    {
        private Mock<IRandom> randomMock;
        private NameGenerator nameGenerator;

        [SetUp]
        public void SetUp()
        {
            randomMock = new();
            randomMock.Setup(x => x.Next(It.IsAny<System.Range>()))
                .Returns((System.Range range) => range.Start.Value);
            randomMock.Setup(x => x.NextElement(It.IsAny<IReadOnlyList<char>>()))
                .Returns((IReadOnlyList<char> list) => list[0]);
            randomMock.Setup(x => x.NextElement(It.IsAny<IReadOnlyList<Action<StringBuilder>>>()))
                .Returns((IReadOnlyList<Action<StringBuilder>> actions) => actions[0]);

            nameGenerator = new(randomMock.Object);
        }

        [Test]
        public void GetName_MakeEveryWordStartUpper([Range(1, 5)] int wordCount)
        {
            var nameProps = new NameGenerationParams(wordCount..wordCount, 1..1);

            var name = nameGenerator.GetName(nameProps);

            name.Count(x => char.IsUpper(x)).Should().Be(wordCount);
        }

        [Test]
        public void GetName_MakeCorrectWordCount([Range(1, 5)] int wordCount)
        {
            var nameProps = new NameGenerationParams(wordCount..wordCount, 1..1);

            var name = nameGenerator.GetName(nameProps);

            name.Count(x => char.IsWhiteSpace(x)).Should().Be(wordCount - 1);
        }
    }
}
