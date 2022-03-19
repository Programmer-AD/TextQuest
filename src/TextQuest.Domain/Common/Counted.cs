namespace TextQuest.Domain.Common
{
    public class Counted<T>
    {
        public T Value { get; set; }
        public uint Count { get; set; }

        public Counted() { }

        public Counted(T item, uint count)
        {
            Value = item;
            Count = count;
        }

        public void Deconstruct(out T item, out uint count)
        {
            item = Value;
            count = Count;
        }
    }
}
