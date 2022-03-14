namespace TextQuest.Application.Interfaces
{
    public interface IRandomGenerator
    {
        int Next(Range range);
        T NextElement<T>(IReadOnlyList<T> list);
    }
}
