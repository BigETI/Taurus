using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Taurus.Collections
{
    /// <summary>
    /// A class that describes a fast enumerable concurrent queue
    /// </summary>
    /// <typeparam name="T">Element type</typeparam>
    public sealed class FastEnumerableConcurrentQueue<T> : IProducerConsumerCollection<T>, IReadOnlyCollection<T>
    {
        /// <summary>
        /// Concurrent queue
        /// </summary>
        private readonly ConcurrentQueue<T> concurrentQueue;

        /// <summary>
        /// An alias to the concurrent queue
        /// </summary>
        private readonly IProducerConsumerCollection<T> producerConsumerCollectionInterface;

        /// <summary>
        /// Are fast enumerable values outdated
        /// </summary>
        private volatile bool areFastEnumerableValuesOutdated;

        /// <summary>
        /// Fast enumerable values
        /// </summary>
        private volatile T[] fastEnumerableValues = Array.Empty<T>();

        /// <summary>
        /// Constructs a new fast enumrable concurrent queue
        /// </summary>
        public FastEnumerableConcurrentQueue()
        {
            concurrentQueue = new ConcurrentQueue<T>();
            producerConsumerCollectionInterface = concurrentQueue;
        }

        /// <summary>
        /// Constructs a new fast enumerable concurrent queue and copies the contents of the specified collection into this fast enumerable concurrent queue
        /// </summary>
        /// <param name="collection">Collection to copy the contents of</param>
        public FastEnumerableConcurrentQueue(IEnumerable<T> collection)
        {
            concurrentQueue = new ConcurrentQueue<T>(collection);
            producerConsumerCollectionInterface = concurrentQueue;
        }

        /// <summary>
        /// Number of elements within this collection
        /// </summary>
        public int Count => concurrentQueue.Count;

        /// <summary>
        /// Is this collection empty
        /// </summary>
        public bool IsEmpty => concurrentQueue.IsEmpty;

        /// <summary>
        /// Is this collection synchronized
        /// </summary>
        bool ICollection.IsSynchronized => producerConsumerCollectionInterface.IsSynchronized;

        /// <summary>
        /// Synchronized root object
        /// </summary>
        object ICollection.SyncRoot => producerConsumerCollectionInterface.SyncRoot;

        /// <summary>
        /// Clears this collection
        /// </summary>
        public void Clear()
        {
            if (concurrentQueue.Count > 0)
            {
                concurrentQueue.Clear();
                areFastEnumerableValuesOutdated = true;
            }
        }

        /// <summary>
        /// Copies the contents of this collection to the specified array
        /// </summary>
        /// <param name="array">Array to copy the contents to</param>
        /// <param name="index">Starting Array index</param>
        public void CopyTo(T[] array, int index) => concurrentQueue.CopyTo(array, index);

        /// <summary>
        /// Enqueues the specified item to this fast enumerable concurrent queue
        /// </summary>
        /// <param name="item">Item to be enqueued</param>
        public void Enqueue(T item)
        {
            concurrentQueue.Enqueue(item);
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
                fastEnumerableValues = concurrentQueue.ToArray();
                areFastEnumerableValuesOutdated = false;
            }
            foreach (T value in fastEnumerableValues)
            {
                yield return value;
            }
        }

        /// <summary>
        /// Creates a new array from the contents of this collection
        /// </summary>
        /// <returns>Newly created array from the contents of this collection</returns>
        public T[] ToArray() => concurrentQueue.ToArray();

        /// <summary>
        /// Tries to dequeue an item from this fast enumerable concurrent queue
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public bool TryDequeue(out T result)
        {
            bool ret = concurrentQueue.TryDequeue(out result);
            areFastEnumerableValuesOutdated = ret || areFastEnumerableValuesOutdated;
            return ret;
        }

        /// <summary>
        /// Tries to peek the next item from this fast enumerable concurrent queue
        /// </summary>
        /// <param name="result">Next element</param>
        /// <returns>"true" if the next element was successfully peeked from this collection, otherwise "false"</returns>
        public bool TryPeek(out T result) => concurrentQueue.TryPeek(out result);

        /// <summary>
        /// Copies the contents of this collection to the specified array
        /// </summary>
        /// <param name="array">Array to copy the contents to</param>
        /// <param name="index">Starting array index</param>
        void ICollection.CopyTo(Array array, int index) => producerConsumerCollectionInterface.CopyTo(array, index);

        /// <summary>
        /// Gets the enumerator of this collection
        /// </summary>
        /// <returns>Enumerator of this collection</returns>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// Tries to add the specified item to this collection
        /// </summary>
        /// <param name="item">Item to be added</param>
        /// <returns>"true" if the specified item has been successfully added to this collection, otherwise "false"</returns>
        bool IProducerConsumerCollection<T>.TryAdd(T item)
        {
            bool ret = producerConsumerCollectionInterface.TryAdd(item);
            areFastEnumerableValuesOutdated = ret || areFastEnumerableValuesOutdated;
            return ret;
        }

        /// <summary>
        /// Tries to take an item from this collection
        /// </summary>
        /// <param name="item">Item to be taken</param>
        /// <returns>"true" if an item has been successfully taken out from this collection, otherwise "false"</returns>
        bool IProducerConsumerCollection<T>.TryTake(out T item)
        {
            bool ret = producerConsumerCollectionInterface.TryTake(out item);
            areFastEnumerableValuesOutdated = ret || areFastEnumerableValuesOutdated;
            return ret;
        }
    }
}
