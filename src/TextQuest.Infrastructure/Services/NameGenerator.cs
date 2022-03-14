using System.Text;
using TextQuest.Application.Interfaces;
using TextQuest.Application.Models;

namespace TextQuest.Infrastructure.Services
{
    internal class NameGenerator : INameGenerator
    {
        private static readonly char[] Vowels = "аеиоя".ToCharArray();
        private static readonly char[] Consonants = "бвгдклмнпрст".ToCharArray();

        private readonly IRandom random;
        private readonly Action<StringBuilder>[] partAdders;

        public NameGenerator(IRandom random)
        {
            this.random = random;
            partAdders = GetPartAdders();
        }

        public string GetName(NameGenerationParams generationParams)
        {
            var wordCount = random.Next(generationParams.WordCount);
            var stringBuilder = new StringBuilder();

            for (int i = 0; i < wordCount; i++)
            {
                if (i != 0)
                {
                    stringBuilder.Append(' ');
                }
                var firstLetter = stringBuilder.Length;

                var partCount = random.Next(generationParams.PartCount);
                AppendWord(stringBuilder, partCount);

                stringBuilder[firstLetter] = char.ToUpper(stringBuilder[firstLetter]);
            }

            var result = stringBuilder.ToString();
            return result;

        }

        private void AppendWord(StringBuilder stringBuilder, int partCount)
        {
            for (int i = 0; i < partCount; i++)
            {
                AppendPart(stringBuilder);
            }
        }

        private void AppendPart(StringBuilder stringBuilder)
        {
            var partAdder = random.NextElement(partAdders);
            partAdder(stringBuilder);
        }

        private char RandomVowel => random.NextElement(Vowels);
        private char RandomConsonant => random.NextElement(Consonants);

        private Action<StringBuilder>[] GetPartAdders()
        {
            var partAdders = new Action<StringBuilder>[]
            {
                x => x.Append(RandomConsonant).Append(RandomVowel),
                x => x.Append(RandomConsonant).Append(RandomVowel).Append(RandomConsonant),
            };
            return partAdders;
        }
    }
}
