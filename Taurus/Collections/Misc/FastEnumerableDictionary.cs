using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Taurus.Collections
{
    public sealed class FastEnumerableDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IReadOnlyDictionary<TKey, TValue>
    {
        private readonly Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();

        private readonly IDictionary<TKey, TValue> dictionaryInterface;

        private bool areFastEnumerableKeyValuePairsOutdated;

        private bool areFastEnumerableValuesOutdated;

        private bool areFastEnumerableKeysOutdated;

        private KeyValuePair<TKey, TValue>[] fastEnumerableKeyValuePairs = Array.Empty<KeyValuePair<TKey, TValue>>();

        private TKey[] fastEnumerableKeys = Array.Empty<TKey>();

        private TValue[] fastEnumerableValues = Array.Empty<TValue>();

        public FastEnumerableDictionary()
        {
            dictionary = new Dictionary<TKey, TValue>();
            dictionaryInterface = dictionary;
        }

        public FastEnumerableDictionary(IDictionary<TKey, TValue> dictionary)
        {
            this.dictionary = new Dictionary<TKey, TValue>(dictionary);
            dictionaryInterface = this.dictionary;
        }

        public FastEnumerableDictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection)
        {
            dictionary = new Dictionary<TKey, TValue>(collection);
            dictionaryInterface = dictionary;
        }

        public FastEnumerableDictionary(IEqualityComparer<TKey> comparer)
        {
            dictionary = new Dictionary<TKey, TValue>(comparer);
            dictionaryInterface = dictionary;
        }

        public FastEnumerableDictionary(int capacity)
        {
            dictionary = new Dictionary<TKey, TValue>(capacity);
            dictionaryInterface = dictionary;
        }

        public FastEnumerableDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer)
        {
            this.dictionary = new Dictionary<TKey, TValue>(dictionary, comparer);
            dictionaryInterface = this.dictionary;
        }

        public FastEnumerableDictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection, IEqualityComparer<TKey> comparer)
        {
            dictionary = new Dictionary<TKey, TValue>(collection, comparer);
            dictionaryInterface = dictionary;
        }

        public FastEnumerableDictionary(int capacity, IEqualityComparer<TKey> comparer)
        {
            dictionary = new Dictionary<TKey, TValue>(capacity, comparer);
            dictionaryInterface = dictionary;
        }

        public TValue this[TKey key]
        {
            get => dictionary[key];
            set => dictionary[key] = value;
        }

        public ICollection<TKey> Keys
        {
            get
            {
                if (areFastEnumerableKeysOutdated)
                {
                    fastEnumerableKeys = dictionary.Keys.ToArray();
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
                    fastEnumerableValues = dictionary.Values.ToArray();
                    areFastEnumerableValuesOutdated = false;
                }
                return fastEnumerableValues;
            }
        }

        public IEqualityComparer<TKey> Comparer => dictionary.Comparer;

        public int Count => dictionary.Count;

        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => dictionaryInterface.IsReadOnly;

        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => throw new NotImplementedException();

        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => throw new NotImplementedException();

        public void Add(TKey key, TValue value)
        {
            dictionary.Add(key, value);
            areFastEnumerableKeyValuePairsOutdated = true;
            areFastEnumerableKeysOutdated = true;
            areFastEnumerableValuesOutdated = true;
        }

        public void Clear()
        {
            if (dictionary.Count > 0)
            {
                dictionary.Clear();
                areFastEnumerableKeyValuePairsOutdated = true;
                areFastEnumerableKeysOutdated = true;
                areFastEnumerableValuesOutdated = true;
            }
        }

        public bool ContainsKey(TKey key) => dictionary.ContainsKey(key);

        public bool ContainsValue(TValue value) => dictionary.ContainsValue(value);

        public int EnsureCapacity(int capacity) => dictionary.EnsureCapacity(capacity);

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            if (areFastEnumerableKeyValuePairsOutdated)
            {
                fastEnumerableKeyValuePairs = dictionary.ToArray();
                areFastEnumerableKeyValuePairsOutdated = false;
            }
            return new ArrayEnumerator<KeyValuePair<TKey, TValue>>(fastEnumerableKeyValuePairs);
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context) => dictionary.GetObjectData(info, context);

        public void OnDeserialization(object sender) => dictionary.OnDeserialization(sender);

        public bool Remove(TKey key, out TValue value)
        {
            bool ret = dictionary.Remove(key, out value);
            if (ret)
            {
                areFastEnumerableKeyValuePairsOutdated = true;
                areFastEnumerableKeysOutdated = true;
                areFastEnumerableValuesOutdated = true;
            }
            return ret;
        }

        public bool Remove(TKey key)
        {
            bool ret = dictionary.Remove(key);
            if (ret)
            {
                areFastEnumerableKeyValuePairsOutdated = true;
                areFastEnumerableKeysOutdated = true;
                areFastEnumerableValuesOutdated = true;
            }
            return ret;
        }

        public void TrimExcess() => dictionary.TrimExcess();

        public void TrimExcess(int capacity) => dictionary.TrimExcess(capacity);

        public bool TryAdd(TKey key, TValue value)
        {
            bool ret = dictionary.TryAdd(key, value);
            if (ret)
            {
                areFastEnumerableKeyValuePairsOutdated = true;
                areFastEnumerableKeysOutdated = true;
                areFastEnumerableValuesOutdated = true;
            }
            return ret;
        }

        public bool TryGetValue(TKey key, out TValue value) => dictionary.TryGetValue(key, out value);

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
