using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Taurus.Collections
{
    /// <summary>
    /// A class that describes a fast enumerable concurrent bag
    /// </summary>
    /// <typeparam name="T">Element type</typeparam>
    public sealed class FastEnumerableConcurrentBag<T> : IProducerConsumerCollection<T>, IReadOnlyCollection<T>
    {
        /// <summary>
        /// Concurrent bag
        /// </summary>
        private readonly ConcurrentBag<T> concurrentBag;

        /// <summary>
        /// An alias to the concurrent bag field
        /// </summary>
        private readonly IProducerConsumerCollection<T> producerConsumerInterface;

        /// <summary>
        /// Are fast enumerable values outdated
        /// </summary>
        private volatile bool areFastEnumerableValuesOutdated;

        /// <summary>
        /// Fast enumerable values
        /// </summary>
        private volatile T[] fastEnumerableValues = Array.Empty<T>();

        /// <summary>
        /// Number of elements inside this collection
        /// </summary>
        public int Count => concurrentBag.Count;

        /// <summary>
        /// Is this collection empty
        /// </summary>
        public bool IsEmpty => concurrentBag.IsEmpty;

        /// <summary>
        /// Is collection synchronized
        /// </summary>
        bool ICollection.IsSynchronized => producerConsumerInterface.IsSynchronized;

        /// <summary>
        /// An object that can be used to have synchronized access
        /// </summary>
        object ICollection.SyncRoot => producerConsumerInterface.SyncRoot;

        /// <summary>
        /// Constructs a new fast enumerable concurrent bag
        /// </summary>
        public FastEnumerableConcurrentBag()
        {
            concurrentBag = new ConcurrentBag<T>();
            producerConsumerInterface = concurrentBag;
        }

        /// <summary>
        /// Constructs a new fast enumerable concurrent bag with the specified collection values to be copied from
        /// </summary>
        /// <param name="collection">Collection values to be copied from</param>
        public FastEnumerableConcurrentBag(IEnumerable<T> collection)
        {
            concurrentBag = new ConcurrentBag<T>(collection);
            producerConsumerInterface = concurrentBag;
        }

        /// <summary>
        /// Adds a new element to this collection
        /// </summary>
        /// <param name="item">Element item</param>
        public void Add(T item)
        {
            concurrentBag.Add(item);
            areFastEnumerableValuesOutdated = true;
        }

        /// <summary>
        /// Clears this collection
        /// </summary>
        public void Clear()
        {
            if (concurrentBag.Count > 0)
            {
                concurrentBag.Clear();
                areFastEnumerableValuesOutdated = true;
            }
        }

        /// <summary>
        /// Copies the specified elements into this collection
        /// </summary>
        /// <param name="array">Array to copy elements from</param>
        /// <param name="index">Starting index to copy elements from</param>
        public void CopyTo(T[] array, int index) => concurrentBag.CopyTo(array, index);

        /// <summary>
        /// Creates a new enumerator for this collection
        /// </summary>
        /// <returns>Newly created fast enumerable concurrent bag enumerator</returns>
        public IEnumerator<T> GetEnumerator()
        {
            if (areFastEnumerableValuesOutdated)
            {
                fastEnumerableValues = concurrentBag.ToArray();
                areFastEnumerableValuesOutdated = false;
            }
            foreach (T value in fastEnumerableValues)
            {
                yield return value;
            }
        }

        /// <summary>
        /// Creates a new array containing all elements of this collection
        /// </summary>
        /// <returns></returns>
        public T[] ToArray() => concurrentBag.ToArray();

        /// <summary>
        /// Tries to peek an element from this collection
        /// </summary>
        /// <param name="result">Peeked element</param>
        /// <returns>"true" if an element has been successfully peeked, otherwise "false"</returns>
        public bool TryPeek(out T result) => concurrentBag.TryPeek(out result);

        /// <summary>
        /// Tries to take an element out from this collection
        /// </summary>
        /// <param name="result">Taken element</param>
        /// <returns>"true" if an element has been successfully taken out from this collection, otherwise "false"</returns>
        public bool TryTake(out T result)
        {
            bool ret = concurrentBag.TryTake(out result);
            areFastEnumerableValuesOutdated = ret || areFastEnumerableValuesOutdated;
            return ret;
        }

        /// <summary>
        /// Copies the specified elements into this collection
        /// </summary>
        /// <param name="array">Array elements to copy from</param>
        /// <param name="index">Starting index to copy elements from</param>
        void ICollection.CopyTo(Array array, int index) => producerConsumerInterface.CopyTo(array, index);

        /// <summary>
        /// Gets an enumerator of this collection
        /// </summary>
        /// <returns>Enumerator</returns>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// Tries to add a new element into this collection
        /// </summary>
        /// <param name="item">Element item</param>
        /// <returns>"true" if the specified element has been successfully added into this collection, otherwise "false"</returns>
        bool IProducerConsumerCollection<T>.TryAdd(T item)
        {
            bool ret = producerConsumerInterface.TryAdd(item);
            areFastEnumerableValuesOutdated = ret || areFastEnumerableValuesOutdated;
            return ret;
        }
    }
}
