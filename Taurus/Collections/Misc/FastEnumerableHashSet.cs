using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Taurus.Collections
{
    /// <summary>
    /// A class that describes a fast enumerable hash set
    /// </summary>
    /// <typeparam name="T">Element type</typeparam>
    public sealed class FastEnumerableHashSet<T> : ISet<T>, IReadOnlyCollection<T>
    {
        /// <summary>
        /// Hash set
        /// </summary>
        private readonly HashSet<T> hashSet;

        /// <summary>
        /// Set interface
        /// </summary>
        private readonly ISet<T> setInterface;

        /// <summary>
        /// Are fast enumerable values outdated
        /// </summary>
        private bool areFastEnumerableValuesOutdated;

        /// <summary>
        /// Fast enumerable values
        /// </summary>
        private T[] fastEnumerableValues = Array.Empty<T>();

        /// <summary>
        /// Constructs a new fast enumerable hash set
        /// </summary>
        public FastEnumerableHashSet() => setInterface = hashSet = new HashSet<T>();

        /// <summary>
        /// Constructs a new fast enumerable hash set
        /// </summary>
        /// <param name="collection">Collection to be inserted</param>
        public FastEnumerableHashSet(IEnumerable<T> collection)
        {
            hashSet = new HashSet<T>(collection);
            setInterface = hashSet;
        }

        /// <summary>
        /// Constructs a new fast enumerable hash set
        /// </summary>
        /// <param name="comparer">Comparer used to compare elements</param>
        public FastEnumerableHashSet(IEqualityComparer<T> comparer)
        {
            hashSet = new HashSet<T>(comparer);
            setInterface = hashSet;
        }

        /// <summary>
        /// Constructs a new fast enumerable hash set
        /// </summary>
        /// <param name="capacity">Initial capacity of the internal hash set</param>
        public FastEnumerableHashSet(int capacity)
        {
            hashSet = new HashSet<T>(capacity);
            setInterface = hashSet;
        }

        /// <summary>
        /// Constructs a new fast enumerable hash set
        /// </summary>
        /// <param name="collection">Collection to be inserted</param>
        /// <param name="comparer">Comparer used to compare elements</param>
        public FastEnumerableHashSet(IEnumerable<T> collection, IEqualityComparer<T> comparer)
        {
            hashSet = new HashSet<T>(collection, comparer);
            setInterface = hashSet;
        }

        /// <summary>
        /// Constructs a new fast enumerable hash set
        /// </summary>
        /// <param name="capacity">Initial capacity of the internal hash set</param>
        /// <param name="comparer">Comparer used to compare elements</param>
        public FastEnumerableHashSet(int capacity, IEqualityComparer<T> comparer)
        {
            hashSet = new HashSet<T>(capacity, comparer);
            setInterface = hashSet;
        }

        /// <summary>
        /// Number of elements inside this collection
        /// </summary>
        public int Count => hashSet.Count;

        /// <summary>
        /// Comparer used to compare elements
        /// </summary>
        public IEqualityComparer<T> Comparer => hashSet.Comparer;

        /// <summary>
        /// Is collection read-only
        /// </summary>
        bool ICollection<T>.IsReadOnly => setInterface.IsReadOnly;

        /// <summary>
        /// Adds the specified element to this collection
        /// </summary>
        /// <param name="item">Element to be added</param>
        /// <returns>"true" if the specified element has been successfully added, otherwise "false"</returns>
        public bool Add(T item)
        {
            bool ret = hashSet.Add(item);
            areFastEnumerableValuesOutdated = ret || areFastEnumerableValuesOutdated;
            return ret;
        }

        /// <summary>
        /// Clears this collection
        /// </summary>
        public void Clear()
        {
            if (hashSet.Count > 0)
            {
                hashSet.Clear();
                areFastEnumerableValuesOutdated = true;
            }
        }

        /// <summary>
        /// Checks if the specified item is contained inside this collection
        /// </summary>
        /// <param name="item">Item</param>
        /// <returns>"true" if the specified item is contained inside this collection, otherwise "false"</returns>
        public bool Contains(T item) => hashSet.Contains(item);

        /// <summary>
        /// Copies the specified array to this collection
        /// </summary>
        /// <param name="array">Array to be copied</param>
        public void CopyTo(T[] array) => hashSet.CopyTo(array);

        /// <summary>
        /// Copies the specified array to this collection
        /// </summary>
        /// <param name="array">Array to be copied</param>
        /// <param name="arrayIndex">Starting array index to copy contents from</param>
        public void CopyTo(T[] array, int arrayIndex) => hashSet.CopyTo(array, arrayIndex);

        /// <summary>
        /// Copies the specified array to this collection
        /// </summary>
        /// <param name="array">Array to be copied</param>
        /// <param name="arrayIndex">Starting array index to copy contents from</param>
        /// <param name="count">Number of elements to copy from the specified array</param>
        public void CopyTo(T[] array, int arrayIndex, int count) => hashSet.CopyTo(array, arrayIndex, count);

        /// <summary>
        /// Ensures that the specified capacity is set
        /// </summary>
        /// <param name="capacity">Capacity</param>
        /// <returns>New capacity</returns>
        public int EnsureCapacity(int capacity) => hashSet.EnsureCapacity(capacity);

        /// <summary>
        /// Exludes all elements specified inside the other collection from its own collection
        /// </summary>
        /// <param name="other">Other collection</param>
        public void ExceptWith(IEnumerable<T> other)
        {
            hashSet.ExceptWith(other);
            areFastEnumerableValuesOutdated = true;
        }

        /// <summary>
        /// Gets the enumerator of this collection
        /// </summary>
        /// <returns>Enumerator of this collection</returns>
        public IEnumerator<T> GetEnumerator()
        {
            if (areFastEnumerableValuesOutdated)
            {
                fastEnumerableValues = hashSet.ToArray();
                areFastEnumerableValuesOutdated = false;
            }
            foreach (T value in fastEnumerableValues)
            {
                yield return value;
            }
        }

        /// <summary>
        /// Gets the object data for serialization
        /// </summary>
        /// <param name="info">Serialization information</param>
        /// <param name="context">Streaming context</param>
        public void GetObjectData(SerializationInfo info, StreamingContext context) => hashSet.GetObjectData(info, context);

        /// <summary>
        /// Keeps all the elements that intersect with the other specified collection
        /// </summary>
        /// <param name="other">Other collection</param>
        public void IntersectWith(IEnumerable<T> other)
        {
            hashSet.IntersectWith(other);
            areFastEnumerableValuesOutdated = true;
        }

        /// <summary>
        /// Checks if the specified collection is a proper subset of this collection
        /// </summary>
        /// <param name="other">Other collection</param>
        /// <returns>"true" if the other specified collection is a proper subset of this collection, otherwise "false"</returns>
        public bool IsProperSubsetOf(IEnumerable<T> other) => hashSet.IsProperSubsetOf(other);

        /// <summary>
        /// Checks if the specified collection is a proper superset of this collection
        /// </summary>
        /// <param name="other">Other collection</param>
        /// <returns>"true" if the other specified collection is a proper superset of this collection, otherwise "false"</returns>
        public bool IsProperSupersetOf(IEnumerable<T> other) => hashSet.IsProperSupersetOf(other);

        /// <summary>
        /// Checks if the specified collection is a subset of this collection
        /// </summary>
        /// <param name="other">Other collection</param>
        /// <returns>"true" if the other specified collection is a subset of this collection, otherwise "false"</returns>
        public bool IsSubsetOf(IEnumerable<T> other) => hashSet.IsSubsetOf(other);

        /// <summary>
        /// Checks if the specified collection is a superset of this collection
        /// </summary>
        /// <param name="other">Other collection</param>
        /// <returns>"true" if the other specified collection is a superset of this collection, otherwise "false"</returns>
        public bool IsSupersetOf(IEnumerable<T> other) => hashSet.IsSupersetOf(other);

        /// <summary>
        /// Gets invoked when this collection has been deserialized
        /// </summary>
        /// <param name="sender"></param>
        public void OnDeserialization(object sender) => hashSet.OnDeserialization(sender);

        /// <summary>
        /// Overlaps the elements from the other specified collection
        /// </summary>
        /// <param name="other">Other collection</param>
        /// <returns>"true" if this collection has been modified, otherwise "false"</returns>
        public bool Overlaps(IEnumerable<T> other)
        {
            bool ret = hashSet.Overlaps(other);
            areFastEnumerableValuesOutdated = ret || areFastEnumerableValuesOutdated;
            return ret;
        }

        /// <summary>
        /// Removes the specified element from this collection
        /// </summary>
        /// <param name="item">Element</param>
        /// <returns>"true" if the specified element has been removed from this collection, otherwise "false"</returns>
        public bool Remove(T item)
        {
            bool ret = hashSet.Remove(item);
            areFastEnumerableValuesOutdated = ret || areFastEnumerableValuesOutdated;
            return ret;
        }

        /// <summary>
        /// Removes elements where the predicate matches
        /// </summary>
        /// <param name="match">Predicate that matches with the elements to be removed</param>
        /// <returns>Number of elements removed</returns>
        public int RemoveWhere(Predicate<T> match)
        {
            int ret = hashSet.RemoveWhere(match);
            areFastEnumerableValuesOutdated = (ret > 0) || areFastEnumerableValuesOutdated;
            return ret;
        }

        /// <summary>
        /// Checks if the specified collection is equal to this collection
        /// </summary>
        /// <param name="other">Other collection</param>
        /// <returns>"true" if the other specified collection equals to this collection, otherwise "false"</returns>
        public bool SetEquals(IEnumerable<T> other) => hashSet.SetEquals(other);

        /// <summary>
        /// Removes the elements that are only present inside the other collection or this collection, not both
        /// </summary>
        /// <param name="other">Other collection</param>
        public void SymmetricExceptWith(IEnumerable<T> other)
        {
            hashSet.SymmetricExceptWith(other);
            areFastEnumerableValuesOutdated = true;
        }

        /// <summary>
        /// Trims the current capacity of this collection
        /// </summary>
        public void TrimExcess() => hashSet.TrimExcess();

        /// <summary>
        /// Tries to get the value equal to the specified value
        /// </summary>
        /// <param name="equalValue">Value used to find the actual element</param>
        /// <param name="actualValue">Actually found element</param>
        /// <returns>"true" if the actual element has been found, otherwise "false"</returns>
        public bool TryGetValue(T equalValue, out T actualValue) => hashSet.TryGetValue(equalValue, out actualValue);

        /// <summary>
        /// Adds elements from the other specified collection that are missing inside this collection
        /// </summary>
        /// <param name="other">Other collection</param>
        public void UnionWith(IEnumerable<T> other)
        {
            hashSet.UnionWith(other);
            areFastEnumerableValuesOutdated = true;
        }

        /// <summary>
        /// Adds the specified element to this collection
        /// </summary>
        /// <param name="item">Element</param>
        void ICollection<T>.Add(T item)
        {
            setInterface.Add(item);
            areFastEnumerableValuesOutdated = true;
        }

        /// <summary>
        /// Gets the enumerator of this collection
        /// </summary>
        /// <returns>Enumerator of this collection</returns>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
