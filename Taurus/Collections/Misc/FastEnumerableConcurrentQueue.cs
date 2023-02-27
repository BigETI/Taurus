using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Taurus.Collections
{
    public sealed class FastEnumerableConcurrentQueue<T> : IProducerConsumerCollection<T>, IReadOnlyCollection<T>
    {
        private readonly ConcurrentQueue<T> concurrentQueue;

        private readonly IProducerConsumerCollection<T> producerConsumerCollectionInterface;

        private volatile bool areFastEnumerableValuesOutdated;

        private volatile T[] fastEnumerableValues = Array.Empty<T>();

        public FastEnumerableConcurrentQueue()
        {
            concurrentQueue = new ConcurrentQueue<T>();
            producerConsumerCollectionInterface = concurrentQueue;
        }

        public FastEnumerableConcurrentQueue(IEnumerable<T> collection)
        {
            concurrentQueue = new ConcurrentQueue<T>(collection);
            producerConsumerCollectionInterface = concurrentQueue;
        }

        public int Count => concurrentQueue.Count;

        public bool IsEmpty => concurrentQueue.IsEmpty;

        bool ICollection.IsSynchronized => producerConsumerCollectionInterface.IsSynchronized;

        object ICollection.SyncRoot => producerConsumerCollectionInterface.SyncRoot;

        public void Clear()
        {
            if (concurrentQueue.Count > 0)
            {
                concurrentQueue.Clear();
                areFastEnumerableValuesOutdated = true;
            }
        }

        public void CopyTo(T[] array, int index) => concurrentQueue.CopyTo(array, index);

        public void Enqueue(T item)
        {
            concurrentQueue.Enqueue(item);
            areFastEnumerableValuesOutdated = true;
        }

        public IEnumerator<T> GetEnumerator()
        {
            if (areFastEnumerableValuesOutdated)
            {
                fastEnumerableValues = concurrentQueue.ToArray();
                areFastEnumerableValuesOutdated = false;
            }
            return new ArrayEnumerator<T>(fastEnumerableValues);
        }

        public T[] ToArray() => concurrentQueue.ToArray();

        public bool TryDequeue(out T result)
        {
            bool ret = concurrentQueue.TryDequeue(out result);
            areFastEnumerableValuesOutdated = ret || areFastEnumerableValuesOutdated;
            return ret;
        }

        public bool TryPeek(out T result) => concurrentQueue.TryPeek(out result);

        void ICollection.CopyTo(Array array, int index) => producerConsumerCollectionInterface.CopyTo(array, index);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        bool IProducerConsumerCollection<T>.TryAdd(T item)
        {
            bool ret = producerConsumerCollectionInterface.TryAdd(item);
            areFastEnumerableValuesOutdated = ret || areFastEnumerableValuesOutdated;
            return ret;
        }

        bool IProducerConsumerCollection<T>.TryTake(out T item)
        {
            bool ret = producerConsumerCollectionInterface.TryTake(out item);
            areFastEnumerableValuesOutdated = ret || areFastEnumerableValuesOutdated;
            return ret;
        }
    }
}
