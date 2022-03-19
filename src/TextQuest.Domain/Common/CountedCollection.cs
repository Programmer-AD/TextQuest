using System.Collections;

namespace TextQuest.Domain.Common
{
    public class CountedCollection<T> : ICollection<Counted<T>>
    {
        private readonly Dictionary<T, Counted<T>> items;

        public CountedCollection()
        {
            items = new();
        }

        public int Count => items.Count;
        public bool IsReadOnly => false;

        public void Add(Counted<T> item)
        {
            if (item.Count > 0)
            {
                if (items.TryGetValue(item.Value, out var existing))
                {
                    existing.Count += item.Count;
                }
                else
                {
                    items.Add(item.Value, new(item.Value, item.Count));
                }
            }
        }

        public void Clear()
        {
            items.Clear();
        }

        public bool Contains(Counted<T> item)
        {
            return
                items.TryGetValue(item.Value, out var existing)
                && existing.Count >= item.Count;
        }

        public void CopyTo(Counted<T>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<Counted<T>> GetEnumerator()
        {
            return items.Select(x => x.Value).GetEnumerator();
        }

        public bool Remove(Counted<T> item)
        {
            if (item.Count > 0
                && items.TryGetValue(item.Value, out var existing)
                && existing.Count >= item.Count)
            {
                existing.Count -= item.Count;
                RemoveIfEmpty(existing);
                return true;
            }
            return false;
        }

        private void RemoveIfEmpty(Counted<T> counted)
        {
            if (counted.Count == 0)
            {
                items.Remove(counted.Value);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
