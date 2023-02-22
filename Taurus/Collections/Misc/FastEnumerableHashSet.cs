using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Taurus.Collections
{
    public sealed class FastEnumerableHashSet<T> : ISet<T>, IReadOnlyCollection<T>
    {
        private readonly HashSet<T> hashSet;

        private readonly ISet<T> setInterface;

        private bool areFastEnumerableValuesOutdated;

        private T[] fastEnumerableValues = Array.Empty<T>();

        public FastEnumerableHashSet() => setInterface = hashSet = new HashSet<T>();

        public FastEnumerableHashSet(IEnumerable<T> collection)
        {
            hashSet = new HashSet<T>(collection);
            setInterface = hashSet;
        }

        public FastEnumerableHashSet(IEqualityComparer<T> comparer)
        {
            hashSet = new HashSet<T>(comparer);
            setInterface = hashSet;
        }

        public FastEnumerableHashSet(int capacity)
        {
            hashSet = new HashSet<T>(capacity);
            setInterface = hashSet;
        }

        public FastEnumerableHashSet(IEnumerable<T> collection, IEqualityComparer<T> comparer)
        {
            hashSet = new HashSet<T>(collection, comparer);
            setInterface = hashSet;
        }

        public FastEnumerableHashSet(int capacity, IEqualityComparer<T> comparer)
        {
            hashSet = new HashSet<T>(capacity, comparer);
            setInterface = hashSet;
        }

        public int Count => hashSet.Count;

        public IEqualityComparer<T> Comparer => hashSet.Comparer;

        bool ICollection<T>.IsReadOnly => setInterface.IsReadOnly;

        public bool Add(T item)
        {
            bool ret = hashSet.Add(item);
            areFastEnumerableValuesOutdated = ret || areFastEnumerableValuesOutdated;
            return ret;
        }

        public void Clear()
        {
            if (hashSet.Count > 0)
            {
                hashSet.Clear();
                areFastEnumerableValuesOutdated = true;
            }
        }

        public bool Contains(T item) => hashSet.Contains(item);

        public void CopyTo(T[] array) => hashSet.CopyTo(array);

        public void CopyTo(T[] array, int arrayIndex) => hashSet.CopyTo(array, arrayIndex);

        public void CopyTo(T[] array, int arrayIndex, int count) => hashSet.CopyTo(array, arrayIndex, count);

        public int EnsureCapacity(int capacity) => hashSet.EnsureCapacity(capacity);

        public void ExceptWith(IEnumerable<T> other)
        {
            hashSet.ExceptWith(other);
            areFastEnumerableValuesOutdated = true;
        }

        public IEnumerator<T> GetEnumerator()
        {
            if (areFastEnumerableValuesOutdated)
            {
                fastEnumerableValues = hashSet.ToArray();
                areFastEnumerableValuesOutdated = false;
            }
            return new ArrayEnumerator<T>(fastEnumerableValues);
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context) => hashSet.GetObjectData(info, context);

        public void IntersectWith(IEnumerable<T> other)
        {
            hashSet.IntersectWith(other);
            areFastEnumerableValuesOutdated = true;
        }

        public bool IsProperSubsetOf(IEnumerable<T> other) => hashSet.IsProperSubsetOf(other);

        public bool IsProperSupersetOf(IEnumerable<T> other) => hashSet.IsProperSupersetOf(other);

        public bool IsSubsetOf(IEnumerable<T> other) => hashSet.IsSubsetOf(other);

        public bool IsSupersetOf(IEnumerable<T> other) => hashSet.IsSupersetOf(other);

        public void OnDeserialization(object sender) => hashSet.OnDeserialization(sender);

        public bool Overlaps(IEnumerable<T> other)
        {
            bool ret = hashSet.Overlaps(other);
            areFastEnumerableValuesOutdated = ret || areFastEnumerableValuesOutdated;
            return ret;
        }

        public bool Remove(T item)
        {
            bool ret = hashSet.Remove(item);
            areFastEnumerableValuesOutdated = ret || areFastEnumerableValuesOutdated;
            return ret;
        }

        public int RemoveWhere(Predicate<T> match)
        {
            int ret = hashSet.RemoveWhere(match);
            areFastEnumerableValuesOutdated = (ret > 0) || areFastEnumerableValuesOutdated;
            return ret;
        }

        public bool SetEquals(IEnumerable<T> other) => hashSet.SetEquals(other);

        public void SymmetricExceptWith(IEnumerable<T> other)
        {
            hashSet.SymmetricExceptWith(other);
            areFastEnumerableValuesOutdated = true;
        }

        public void TrimExcess() => hashSet.TrimExcess();

        public bool TryGetValue(T equalValue, out T actualValue) => hashSet.TryGetValue(equalValue, out actualValue);

        public void UnionWith(IEnumerable<T> other)
        {
            hashSet.UnionWith(other);
            areFastEnumerableValuesOutdated = true;
        }

        void ICollection<T>.Add(T item)
        {
            setInterface.Add(item);
            areFastEnumerableValuesOutdated = true;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
