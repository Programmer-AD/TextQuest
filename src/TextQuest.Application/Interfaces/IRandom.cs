namespace TextQuest.Application.Interfaces
{
    public interface IRandom
    {
        /// <summary>
        /// Returns random int from range (borders inclusive)
        /// </summary>
        int Next(Range range);
        T NextElement<T>(IReadOnlyList<T> list);
        void Mix<T>(IList<T> list);
    }
}
