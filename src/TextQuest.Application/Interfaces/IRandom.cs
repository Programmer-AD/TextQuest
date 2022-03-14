namespace TextQuest.Application.Interfaces
{
    public interface IRandom
    {
        int Next(Range range);
        T NextElement<T>(IReadOnlyList<T> list);
        void Mix<T>(IList<T> list);
    }
}
