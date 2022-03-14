using TextQuest.Application.Interfaces;

namespace TextQuest.Infrastructure.Services
{
    internal class RandomAdapter : IRandom
    {
        private readonly Random random;

        public RandomAdapter()
        {
            random = new Random();
        }

        public void Mix<T>(IList<T> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                var newPosition = Next(0..(list.Count - 1));
                (list[i], list[newPosition]) = (list[newPosition], list[i]);
            }
        }

        public int Next(Range range)
        {
            var result = random.Next(range.Start.Value, range.End.Value + 1);
            return result;
        }

        public T NextElement<T>(IReadOnlyList<T> list)
        {
            var index = random.Next(list.Count);
            return list[index];
        }
    }
}
