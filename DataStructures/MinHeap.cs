#nullable disable
using System;
using System.Collections.Generic;

namespace MunicipalServicesApp.DataStructures
{
    /// <summary>
    /// Array-backed binary min-heap. The item that compares smallest is always at index 0.
    /// Parent of i is (i - 1) / 2, children are 2i + 1 and 2i + 2.
    /// Push/Pop are O(log n), Peek is O(1), building from a collection is O(n).
    /// </summary>
    public class MinHeap<T>
    {
        private readonly List<T> items = new List<T>();
        private readonly Comparison<T> compare;

        public MinHeap(Comparison<T> compare)
        {
            this.compare = compare ?? throw new ArgumentNullException(nameof(compare));
        }

        public MinHeap(IEnumerable<T> source, Comparison<T> compare) : this(compare)
        {
            items.AddRange(source);
            for (int i = items.Count / 2 - 1; i >= 0; i--) SiftDown(i);
        }

        public int Count => items.Count;
        public bool IsEmpty => items.Count == 0;

        /// <summary>The heap array in storage order (level by level), for display.</summary>
        public IReadOnlyList<T> RawArray => items;

        public void Push(T item)
        {
            items.Add(item);
            SiftUp(items.Count - 1);
        }

        public T Peek()
        {
            if (items.Count == 0) throw new InvalidOperationException("The heap is empty.");
            return items[0];
        }

        public T Pop()
        {
            if (items.Count == 0) throw new InvalidOperationException("The heap is empty.");
            T top = items[0];
            int last = items.Count - 1;
            items[0] = items[last];
            items.RemoveAt(last);
            if (items.Count > 0) SiftDown(0);
            return top;
        }

        public bool TryPop(out T item)
        {
            if (items.Count == 0) { item = default(T); return false; }
            item = Pop();
            return true;
        }

        /// <summary>All items from smallest to largest without changing this heap (heap-sort on a copy).</summary>
        public List<T> ToSortedList()
        {
            var copy = new MinHeap<T>(items, compare);
            var sorted = new List<T>(items.Count);
            while (!copy.IsEmpty) sorted.Add(copy.Pop());
            return sorted;
        }

        /// <summary>True when every parent is no larger than its children.</summary>
        public bool IsValid()
        {
            for (int i = 1; i < items.Count; i++)
                if (compare(items[(i - 1) / 2], items[i]) > 0) return false;
            return true;
        }

        private void SiftUp(int i)
        {
            while (i > 0)
            {
                int parent = (i - 1) / 2;
                if (compare(items[i], items[parent]) >= 0) break;
                Swap(i, parent);
                i = parent;
            }
        }

        private void SiftDown(int i)
        {
            int n = items.Count;
            while (true)
            {
                int left = 2 * i + 1, right = left + 1, smallest = i;
                if (left < n && compare(items[left], items[smallest]) < 0) smallest = left;
                if (right < n && compare(items[right], items[smallest]) < 0) smallest = right;
                if (smallest == i) break;
                Swap(i, smallest);
                i = smallest;
            }
        }

        private void Swap(int a, int b)
        {
            T tmp = items[a];
            items[a] = items[b];
            items[b] = tmp;
        }
    }
}
