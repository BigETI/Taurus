using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Taurus.Collections
{
    /// <summary>
    /// A class that describes a fast enumerable dictionary
    /// </summary>
    /// <typeparam name="TKey">Key type</typeparam>
    /// <typeparam name="TValue">Value type</typeparam>
    public sealed class FastEnumerableDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IReadOnlyDictionary<TKey, TValue>
    {
        /// <summary>
        /// Dictionary
        /// </summary>
        private readonly Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();

        /// <summary>
        /// An alias to the existing dictionary field
        /// </summary>
        private readonly IDictionary<TKey, TValue> dictionaryInterface;

        /// <summary>
        /// Are fast enumerable key and value pairs outdated
        /// </summary>
        private bool areFastEnumerableKeyValuePairsOutdated;

        /// <summary>
        /// Are fast enumerable keys outdated
        /// </summary>
        private bool areFastEnumerableKeysOutdated;

        /// <summary>
        /// Are fast enumerable values outdated
        /// </summary>
        private bool areFastEnumerableValuesOutdated;

        /// <summary>
        /// Fast enumerable key and value pairs
        /// </summary>
        private KeyValuePair<TKey, TValue>[] fastEnumerableKeyValuePairs = Array.Empty<KeyValuePair<TKey, TValue>>();

        /// <summary>
        /// Fast enumerable keys
        /// </summary>
        private TKey[] fastEnumerableKeys = Array.Empty<TKey>();

        /// <summary>
        /// Fast enumerable values
        /// </summary>
        private TValue[] fastEnumerableValues = Array.Empty<TValue>();

        /// <summary>
        /// Getter and setter for the element value using the specified element key
        /// </summary>
        /// <param name="key">Element key</param>
        /// <returns>Element value</returns>
        public TValue this[TKey key]
        {
            get => dictionary[key];
            set
            {
                dictionary[key] = value;
                areFastEnumerableKeyValuePairsOutdated = true;
                areFastEnumerableValuesOutdated = true;
            }
        }

        /// <summary>
        /// Gets the keys of this collection
        /// </summary>
        public ICollection<TKey> Keys
        {
            get
            {
                if (areFastEnumerableKeysOutdated)
                {
                    if (fastEnumerableKeys.Length != dictionary.Keys.Count)
                    {
                        Array.Resize(ref fastEnumerableKeys, dictionary.Keys.Count);
                    }
                    dictionary.Keys.CopyTo(fastEnumerableKeys, 0);
                    areFastEnumerableKeysOutdated = false;
                }
                return fastEnumerableKeys;
            }
        }

        /// <summary>
        /// Gets the values of this collection
        /// </summary>
        public ICollection<TValue> Values
        {
            get
            {
                if (areFastEnumerableValuesOutdated)
                {
                    if (fastEnumerableValues.Length != dictionary.Values.Count)
                    {
                        Array.Resize(ref fastEnumerableValues, dictionary.Values.Count);
                    }
                    dictionary.Values.CopyTo(fastEnumerableValues, 0);
                    areFastEnumerableValuesOutdated = false;
                }
                return fastEnumerableValues;
            }
        }

        /// <summary>
        /// Comparer used to compare keys
        /// </summary>
        public IEqualityComparer<TKey> Comparer => dictionary.Comparer;

        /// <summary>
        /// Number of key value pairs inside this collection
        /// </summary>
        public int Count => dictionary.Count;

        /// <summary>
        /// Is his collection read-only
        /// </summary>
        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => dictionaryInterface.IsReadOnly;

        /// <summary>
        /// Gets the keys of this collection
        /// </summary>
        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => Keys;

        /// <summary>
        /// Gets the values of this collection
        /// </summary>
        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => Values;

        /// <summary>
        /// Constructs a new fast enumerable dictionary
        /// </summary>
        public FastEnumerableDictionary()
        {
            dictionary = new Dictionary<TKey, TValue>();
            dictionaryInterface = dictionary;
        }

        /// <summary>
        /// Constructs a new fast enumerable dictionary with the specified dictionary key value pairs copied
        /// </summary>
        /// <param name="dictionary">Dictionary to copy key value pairs from</param>
        public FastEnumerableDictionary(IDictionary<TKey, TValue> dictionary)
        {
            this.dictionary = new Dictionary<TKey, TValue>(dictionary);
            dictionaryInterface = this.dictionary;
        }

        /// <summary>
        /// Constructs a new fast enumerable dictionary with the specified dictionary key value pairs copied
        /// </summary>
        /// <param name="collection"></param>
        public FastEnumerableDictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection)
        {
            dictionary = new Dictionary<TKey, TValue>(collection);
            dictionaryInterface = dictionary;
        }

        /// <summary>
        /// Constructs a new fast enumerable dictionary with the comparer used to compare keys
        /// </summary>
        /// <param name="comparer">Comparer used to compare keys</param>
        public FastEnumerableDictionary(IEqualityComparer<TKey> comparer)
        {
            dictionary = new Dictionary<TKey, TValue>(comparer);
            dictionaryInterface = dictionary;
        }

        /// <summary>
        /// Constructs a new fast enumerable dictionary with the initial capacity set
        /// </summary>
        /// <param name="capacity">Initial capacity of this collection</param>
        public FastEnumerableDictionary(int capacity)
        {
            dictionary = new Dictionary<TKey, TValue>(capacity);
            dictionaryInterface = dictionary;
        }

        /// <summary>
        /// Constructs a new fast enumerable dictionary with the specified dictionary key value pairs copied and comparer used to compare keys
        /// </summary>
        /// <param name="dictionary">Dictionary to copy key value pairs from</param>
        /// <param name="comparer">Comparer used to compare keys</param>
        public FastEnumerableDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer)
        {
            this.dictionary = new Dictionary<TKey, TValue>(dictionary, comparer);
            dictionaryInterface = this.dictionary;
        }

        /// <summary>
        /// Constructs a new fast enumerable dictionary with the specified collection key value pairs copied and comparer used to compare keys
        /// </summary>
        /// <param name="collection">Collection to copy key value pairs from</param>
        /// <param name="comparer">Comparer used to compare keys</param>
        public FastEnumerableDictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection, IEqualityComparer<TKey> comparer)
        {
            dictionary = new Dictionary<TKey, TValue>(collection, comparer);
            dictionaryInterface = dictionary;
        }

        /// <summary>
        /// Constructs a new fast enumerable dictionary with the initial capacity set and comparer used to compare keys
        /// </summary>
        /// <param name="capacity">Initial capacity of this collection</param>
        /// <param name="comparer">Comparer used to compare keys</param>
        public FastEnumerableDictionary(int capacity, IEqualityComparer<TKey> comparer)
        {
            dictionary = new Dictionary<TKey, TValue>(capacity, comparer);
            dictionaryInterface = dictionary;
        }

        /// <summary>
        /// Adds the specified key value pair to this dictionary
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        public void Add(TKey key, TValue value)
        {
            dictionary.Add(key, value);
            areFastEnumerableKeyValuePairsOutdated = true;
            areFastEnumerableKeysOutdated = true;
            areFastEnumerableValuesOutdated = true;
        }

        /// <summary>
        /// Clears this dictionary
        /// </summary>
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

        /// <summary>
        /// Checks if the specified key is contained within this fast enumerable dictionary
        /// </summary>
        /// <param name="key">Key to be checked</param>
        /// <returns>"true" if the specified key is contained within this dictionary, otherwise "false"</returns>
        public bool ContainsKey(TKey key) => dictionary.ContainsKey(key);

        /// <summary>
        /// Checks if the specified value is contained within this collection
        /// </summary>
        /// <param name="value">Value to be checked</param>
        /// <returns>"true" if the specified value is contained within this dictionary, otherwise "false"</returns>
        public bool ContainsValue(TValue value) => dictionary.ContainsValue(value);

        /// <summary>
        /// Ensures the capacity of this fast enumerable dictionary
        /// </summary>
        /// <param name="capacity">Capacity to be ensured</param>
        /// <returns>The current capacity of this fast enumerable dictionary</returns>
        public int EnsureCapacity(int capacity) => dictionary.EnsureCapacity(capacity);

        /// <summary>
        /// Gets the enumerator of this collection
        /// </summary>
        /// <returns>Enumerator of this collection</returns>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            if (areFastEnumerableKeyValuePairsOutdated)
            {
                if (fastEnumerableKeyValuePairs.Length != dictionary.Count)
                {
                    Array.Resize(ref fastEnumerableKeyValuePairs, dictionary.Count);
                }
                int fast_enumerable_key_value_pair_index = 0;
                foreach (KeyValuePair<TKey, TValue> key_value_pair in dictionary)
                {
                    fastEnumerableKeyValuePairs[fast_enumerable_key_value_pair_index] = key_value_pair;
                    ++fast_enumerable_key_value_pair_index;
                }
                areFastEnumerableKeyValuePairsOutdated = false;
            }
            foreach (KeyValuePair<TKey, TValue> key_value_pair in fastEnumerableKeyValuePairs)
            {
                yield return key_value_pair;
            }
        }

        /// <summary>
        /// Gets the object data of this fast enumerable dictionary for serialization
        /// </summary>
        /// <param name="info">Serialization information</param>
        /// <param name="context">Streaming context</param>
        public void GetObjectData(SerializationInfo info, StreamingContext context) => dictionary.GetObjectData(info, context);

        /// <summary>
        /// Gets invoked when this fast enumerable dictionary has been deserialized
        /// </summary>
        /// <param name="sender">The sender of this event</param>
        public void OnDeserialization(object sender) => dictionary.OnDeserialization(sender);

        /// <summary>
        /// Removes the specified key and associated value from this fast enumerable dictionary
        /// </summary>
        /// <param name="key">Key to be removed</param>
        /// <param name="value">Removed value</param>
        /// <returns>"true" if the specified key with its associated value has been successfully renmoved from this fast enumerable dictionary, otherwise "false"</returns>
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

        /// <summary>
        /// Removes the specified key and associated value from this fast enumerable dictionary
        /// </summary>
        /// <param name="key">Key to be removed</param>
        /// <returns>"true" if the specified key with its associated value has been successfully renmoved from this fast enumerable dictionary, otherwise "false"</returns>
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

        /// <summary>
        /// Sets the capacity of this fast enumerable dictionary to what it would be if it had been originally initialized with all its entries
        /// </summary>
        public void TrimExcess() => dictionary.TrimExcess();

        /// <summary>
        /// Sets the capacity of this dictionary to hold up a specified number of entries without any further expansion of its backing storage
        /// </summary>
        /// <param name="capacity">The new capacity of this fast enumerable dictionary</param>
        public void TrimExcess(int capacity) => dictionary.TrimExcess(capacity);

        /// <summary>
        /// Tries to add the specified key value pair to this fast enumerable dictionary
        /// </summary>
        /// <param name="key">Key to be added</param>
        /// <param name="value">Value to be added</param>
        /// <returns>"true" if the specified key value pair has been successfully added to this fast enumerable dictionary, otherwise "false"</returns>
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

        /// <summary>
        /// Tries to get a value at the specified key
        /// </summary>
        /// <param name="key">Key to get the value at</param>
        /// <param name="value">Retrieved value</param>
        /// <returns>"true" if a value has been retrieved successfully with the specified key, otherwise "false"</returns>
        public bool TryGetValue(TKey key, out TValue value) => dictionary.TryGetValue(key, out value);

        /// <summary>
        /// Adds the specified item to this collection
        /// </summary>
        /// <param name="item">Item to be added</param>
        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
        {
            dictionaryInterface.Add(item);
            areFastEnumerableKeyValuePairsOutdated = true;
            areFastEnumerableKeysOutdated = true;
            areFastEnumerableValuesOutdated = true;
        }

        /// <summary>
        /// Checks if the specified item is contained within this collection
        /// </summary>
        /// <param name="item">Item to be checked</param>
        /// <returns>"true" if the specified item is contained within this collection, otherwise "false"</returns>
        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item) => dictionaryInterface.Contains(item);

        /// <summary>
        /// Copies the contents of this collection to the specified array
        /// </summary>
        /// <param name="array">Array to copy the contents to</param>
        /// <param name="arrayIndex">Starting array index</param>
        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) => dictionaryInterface.CopyTo(array, arrayIndex);

        /// <summary>
        /// Gets the enumerator of this collection
        /// </summary>
        /// <returns>Enumerator of this collection</returns>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// Removes the specified item from this collection
        /// </summary>
        /// <param name="item">Item to be removed</param>
        /// <returns>"true" if the specified item has been successfully removed from this collection, otherwise "false"</returns>
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
