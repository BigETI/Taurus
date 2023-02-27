using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Taurus.Collections
{
    public sealed class FastEnumerableConcurrentDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IReadOnlyDictionary<TKey, TValue>
    {
        private readonly ConcurrentDictionary<TKey, TValue> concurrentDictionary;

        private readonly IDictionary<TKey, TValue> dictionaryInterface;

        private volatile bool areFastEnumerableKeyValuePairsOutdated;

        private volatile bool areFastEnumerableValuesOutdated;

        private volatile bool areFastEnumerableKeysOutdated;

        private volatile KeyValuePair<TKey, TValue>[] fastEnumerableeyValuePairs = Array.Empty<KeyValuePair<TKey, TValue>>();

        private volatile TKey[] fastEnumerableKeys = Array.Empty<TKey>();

        private volatile TValue[] fastEnumerableValues = Array.Empty<TValue>();

        public TValue this[TKey key]
        {
            get => concurrentDictionary[key];
            set
            {
                concurrentDictionary[key] = value;
                areFastEnumerableKeyValuePairsOutdated = true;
                areFastEnumerableValuesOutdated = true;
            }
        }

        public bool IsEmpty => concurrentDictionary.IsEmpty;

        public int Count => concurrentDictionary.Count;

        public ICollection<TKey> Keys
        {
            get
            {
                if (areFastEnumerableKeysOutdated)
                {
                    fastEnumerableKeys = concurrentDictionary.Keys.ToArray();
                    areFastEnumerableKeysOutdated = false;
                }
                return fastEnumerableKeys;
            }
        }

        public ICollection<TValue> Values
        {
            get
            {
                if (areFastEnumerableValuesOutdated)
                {
                    fastEnumerableValues = concurrentDictionary.Values.ToArray();
                    areFastEnumerableValuesOutdated = false;
                }
                return fastEnumerableValues;
            }
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => dictionaryInterface.IsReadOnly;

        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => Keys;

        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => Values;

        public FastEnumerableConcurrentDictionary()
        {
            concurrentDictionary = new ConcurrentDictionary<TKey, TValue>();
            dictionaryInterface = concurrentDictionary;
        }

        public FastEnumerableConcurrentDictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection)
        {
            concurrentDictionary = new ConcurrentDictionary<TKey, TValue>(collection);
            dictionaryInterface = concurrentDictionary;
        }

        public FastEnumerableConcurrentDictionary(IEqualityComparer<TKey> comparer)
        {
            concurrentDictionary = new ConcurrentDictionary<TKey, TValue>(comparer);
            dictionaryInterface = concurrentDictionary;
        }

        public FastEnumerableConcurrentDictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection, IEqualityComparer<TKey> comparer)
        {
            concurrentDictionary = new ConcurrentDictionary<TKey, TValue>(collection, comparer);
            dictionaryInterface = concurrentDictionary;
        }

        public FastEnumerableConcurrentDictionary(int concurrencyLevel, int capacity)
        {
            concurrentDictionary = new ConcurrentDictionary<TKey, TValue>(concurrencyLevel, capacity);
            dictionaryInterface = concurrentDictionary;
        }

        public FastEnumerableConcurrentDictionary(int concurrencyLevel, IEnumerable<KeyValuePair<TKey, TValue>> collection, IEqualityComparer<TKey> comparer)
        {
            concurrentDictionary = new ConcurrentDictionary<TKey, TValue>(concurrencyLevel, collection, comparer);
            dictionaryInterface = concurrentDictionary;
        }

        public FastEnumerableConcurrentDictionary(int concurrencyLevel, int capacity, IEqualityComparer<TKey> comparer)
        {
            concurrentDictionary = new ConcurrentDictionary<TKey, TValue>(concurrencyLevel, capacity, comparer);
            dictionaryInterface = concurrentDictionary;
        }

        public TValue AddOrUpdate<TArg>
        (
            TKey key,
            Func<TKey, TArg, TValue> addValueFactory,
            Func<TKey, TValue, TArg, TValue> updateValueFactory,
            TArg factoryArgument
        )
        {
            TValue ret = concurrentDictionary.AddOrUpdate(key, addValueFactory, updateValueFactory, factoryArgument);
            areFastEnumerableKeyValuePairsOutdated = true;
            areFastEnumerableKeysOutdated = true;
            areFastEnumerableValuesOutdated = true;
            return ret;
        }

        public TValue AddOrUpdate(TKey key, TValue addValue, Func<TKey, TValue, TValue> updateValueFactory)
        {
            TValue ret = concurrentDictionary.AddOrUpdate(key, addValue, updateValueFactory);
            areFastEnumerableKeyValuePairsOutdated = true;
            areFastEnumerableKeysOutdated = true;
            areFastEnumerableValuesOutdated = true;
            return ret;
        }

        public TValue AddOrUpdate(TKey key, Func<TKey, TValue> addValueFactory, Func<TKey, TValue, TValue> updateValueFactory)
        {
            TValue ret = concurrentDictionary.AddOrUpdate(key, addValueFactory, updateValueFactory);
            areFastEnumerableKeyValuePairsOutdated = true;
            areFastEnumerableKeysOutdated = true;
            areFastEnumerableValuesOutdated = true;
            return ret;
        }

        public void Clear()
        {
            if (concurrentDictionary.Count > 0)
            {
                concurrentDictionary.Clear();
                areFastEnumerableKeyValuePairsOutdated = true;
                areFastEnumerableKeysOutdated = true;
                areFastEnumerableValuesOutdated = true;
            }
        }

        public bool ContainsKey(TKey key) => concurrentDictionary.ContainsKey(key);

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            if (areFastEnumerableKeyValuePairsOutdated)
            {
                fastEnumerableeyValuePairs = concurrentDictionary.ToArray();
                areFastEnumerableKeyValuePairsOutdated = false;
            }
            return new ArrayEnumerator<KeyValuePair<TKey, TValue>>(fastEnumerableeyValuePairs);
        }

        public TValue GetOrAdd(TKey key, Func<TKey, TValue> valueFactory)
        {
            TValue ret = concurrentDictionary.GetOrAdd(key, valueFactory);
            areFastEnumerableKeyValuePairsOutdated = true;
            areFastEnumerableKeysOutdated = true;
            areFastEnumerableValuesOutdated = true;
            return ret;
        }

        public TValue GetOrAdd(TKey key, TValue value)
        {
            TValue ret = concurrentDictionary.GetOrAdd(key, value);
            areFastEnumerableKeyValuePairsOutdated = true;
            areFastEnumerableKeysOutdated = true;
            areFastEnumerableValuesOutdated = true;
            return ret;
        }

        public TValue GetOrAdd<TArg>(TKey key, Func<TKey, TArg, TValue> valueFactory, TArg factoryArgument)
        {
            TValue ret = concurrentDictionary.GetOrAdd(key, valueFactory, factoryArgument);
            areFastEnumerableKeyValuePairsOutdated = true;
            areFastEnumerableKeysOutdated = true;
            areFastEnumerableValuesOutdated = true;
            return ret;
        }

        public KeyValuePair<TKey, TValue>[] ToArray() => concurrentDictionary.ToArray();

        public bool TryAdd(TKey key, TValue value)
        {
            bool ret = concurrentDictionary.TryAdd(key, value);
            if (ret)
            {
                areFastEnumerableKeyValuePairsOutdated = true;
                areFastEnumerableKeysOutdated = true;
                areFastEnumerableValuesOutdated = true;
            }
            return ret;
        }

        public bool TryGetValue(TKey key, out TValue value) => concurrentDictionary.TryGetValue(key, out value);

        public bool TryRemove(TKey key, out TValue value)
        {
            bool ret = concurrentDictionary.TryRemove(key, out value);
            if (ret)
            {
                areFastEnumerableKeyValuePairsOutdated = true;
                areFastEnumerableKeysOutdated = true;
                areFastEnumerableValuesOutdated = true;
            }
            return ret;
        }

        public bool TryUpdate(TKey key, TValue newValue, TValue comparisonValue)
        {
            bool ret = concurrentDictionary.TryUpdate(key, newValue, comparisonValue);
            if (ret)
            {
                areFastEnumerableKeyValuePairsOutdated = true;
                areFastEnumerableKeysOutdated = true;
                areFastEnumerableValuesOutdated = true;
            }
            return ret;
        }

        void IDictionary<TKey, TValue>.Add(TKey key, TValue value)
        {
            dictionaryInterface.Add(key, value);
            areFastEnumerableKeyValuePairsOutdated = true;
            areFastEnumerableKeysOutdated = true;
            areFastEnumerableValuesOutdated = true;
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
        {
            dictionaryInterface.Add(item);
            areFastEnumerableKeyValuePairsOutdated = true;
            areFastEnumerableKeysOutdated = true;
            areFastEnumerableValuesOutdated = true;
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item) => dictionaryInterface.Contains(item);

        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) => dictionaryInterface.CopyTo(array, arrayIndex);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        bool IDictionary<TKey, TValue>.Remove(TKey key)
        {
            bool ret = dictionaryInterface.Remove(key);
            if (ret)
            {
                areFastEnumerableKeyValuePairsOutdated = true;
                areFastEnumerableKeysOutdated = true;
                areFastEnumerableValuesOutdated = true;
            }
            return ret;
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            bool ret = dictionaryInterface.Remove(item);
            if (ret)
            {
                areFastEnumerableKeyValuePairsOutdated = true;
                areFastEnumerableKeysOutdated = true;
                areFastEnumerableValuesOutdated = true;
            }
            return ret;
        }
    }
}
