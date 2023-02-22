using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Taurus.Collections
{
    public sealed class FastEnumerableConcurrentBag<T> : IProducerConsumerCollection<T>, IReadOnlyCollection<T>
    {
        private readonly ConcurrentBag<T> concurrentBag;

        private readonly IProducerConsumerCollection<T> producerConsumerInterface;

        private volatile bool areFastEnumerableValuesOutdated;

        private volatile T[] fastEnumerableValues = Array.Empty<T>();

        public FastEnumerableConcurrentBag()
        {
            concurrentBag = new ConcurrentBag<T>();
            producerConsumerInterface = concurrentBag;
        }

        public FastEnumerableConcurrentBag(IEnumerable<T> collection)
        {
            concurrentBag = new ConcurrentBag<T>(collection);
            producerConsumerInterface = concurrentBag;
        }

        public int Count => concurrentBag.Count;


        public bool IsEmpty => concurrentBag.IsEmpty;

        bool ICollection.IsSynchronized => producerConsumerInterface.IsSynchronized;

        object ICollection.SyncRoot => producerConsumerInterface.SyncRoot;

        public void Add(T item)
        {
            concurrentBag.Add(item);
            areFastEnumerableValuesOutdated = true;
        }

        public void Clear()
        {
            if (concurrentBag.Count > 0)
            {
                concurrentBag.Clear();
                areFastEnumerableValuesOutdated = true;
            }
        }

        public void CopyTo(T[] array, int index) => concurrentBag.CopyTo(array, index);

        public IEnumerator<T> GetEnumerator()
        {
            if (areFastEnumerableValuesOutdated)
            {
                fastEnumerableValues = concurrentBag.ToArray();
                areFastEnumerableValuesOutdated = false;
            }
            return new ArrayEnumerator<T>(fastEnumerableValues);
        }

        public T[] ToArray() => concurrentBag.ToArray();

        public bool TryPeek(out T result) => concurrentBag.TryPeek(out result);

        public bool TryTake(out T result)
        {
            bool ret = concurrentBag.TryTake(out result);
            areFastEnumerableValuesOutdated = ret || areFastEnumerableValuesOutdated;
            return ret;
        }

        void ICollection.CopyTo(Array array, int index) => producerConsumerInterface.CopyTo(array, index);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        bool IProducerConsumerCollection<T>.TryAdd(T item)
        {
            bool ret = producerConsumerInterface.TryAdd(item);
            areFastEnumerableValuesOutdated = ret || areFastEnumerableValuesOutdated;
            return ret;
        }
    }
}
