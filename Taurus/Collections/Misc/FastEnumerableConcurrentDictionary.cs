using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Taurus.Collections
{
    /// <summary>
    /// A class that describes a fast enumerable concurrent dictionary
    /// </summary>
    /// <typeparam name="TKey">Key type</typeparam>
    /// <typeparam name="TValue">Value type</typeparam>
    public sealed class FastEnumerableConcurrentDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IReadOnlyDictionary<TKey, TValue>
    {
        /// <summary>
        /// Concurrent dictionary
        /// </summary>
        private readonly ConcurrentDictionary<TKey, TValue> concurrentDictionary;

        /// <summary>
        /// An alias to the concurrent dictionary
        /// </summary>
        private readonly IDictionary<TKey, TValue> dictionaryInterface;

        /// <summary>
        /// Are fast enumerable key value pairs outdated
        /// </summary>
        private volatile bool areFastEnumerableKeyValuePairsOutdated;

        /// <summary>
        /// Are fast enumerable keys outdated
        /// </summary>
        private volatile bool areFastEnumerableKeysOutdated;

        /// <summary>
        /// Are fast enumerable values outdated
        /// </summary>
        private volatile bool areFastEnumerableValuesOutdated;

        /// <summary>
        /// Fast enumerable key value pairs
        /// </summary>
        private volatile KeyValuePair<TKey, TValue>[] fastEnumerableKeyValuePairs = Array.Empty<KeyValuePair<TKey, TValue>>();

        /// <summary>
        /// Fast enumerable keys
        /// </summary>
        private volatile TKey[] fastEnumerableKeys = Array.Empty<TKey>();

        /// <summary>
        /// Fast enumerable values
        /// </summary>
        private volatile TValue[] fastEnumerableValues = Array.Empty<TValue>();

        /// <summary>
        /// Getter and setter for a value specified by its key
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>Value</returns>
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

        /// <summary>
        /// Is this collection empty
        /// </summary>
        public bool IsEmpty => concurrentDictionary.IsEmpty;

        /// <summary>
        /// Number of elements inside this collection
        /// </summary>
        public int Count => concurrentDictionary.Count;

        /// <summary>
        /// Keys of this collection
        /// </summary>
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

        /// <summary>
        /// Values of this collection
        /// </summary>
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

        /// <summary>
        /// Is this collection read-only
        /// </summary>
        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => dictionaryInterface.IsReadOnly;

        /// <summary>
        /// Keys of this collection
        /// </summary>
        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => Keys;

        /// <summary>
        /// Values of this collection
        /// </summary>
        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => Values;

        private readonly Func<TValue, TValue, bool> isValueEqualTo =
            typeof(IEquatable<TValue>).IsAssignableFrom(typeof(TValue)) ?
                ((oldValue, newValue) => (IEquatable<TValue>)oldValue! == (IEquatable<TValue>)newValue!) :
                (
                    typeof(TValue).IsValueType ?
                        (Func<TValue, TValue, bool>)((oldValue, newValue) => Equals(oldValue, newValue)) :
                        (oldValue, newValue) => ReferenceEquals(oldValue, newValue)
                );

        /// <summary>
        /// Constructs a new fast enumerable concurrent dictionary
        /// </summary>
        public FastEnumerableConcurrentDictionary()
        {
            concurrentDictionary = new ConcurrentDictionary<TKey, TValue>();
            dictionaryInterface = concurrentDictionary;
        }

        /// <summary>
        /// Constructs a new fast enumerable dictionary with the specified collection key value pairs to copy from
        /// </summary>
        /// <param name="collection">Collection key value pairs to copy from</param>
        public FastEnumerableConcurrentDictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection)
        {
            concurrentDictionary = new ConcurrentDictionary<TKey, TValue>(collection);
            dictionaryInterface = concurrentDictionary;
        }

        /// <summary>
        /// Constructs a new fast enumerable dictionary with the specified comparer used to compare keys
        /// </summary>
        /// <param name="comparer">Comparer used to compare keys</param>
        public FastEnumerableConcurrentDictionary(IEqualityComparer<TKey> comparer)
        {
            concurrentDictionary = new ConcurrentDictionary<TKey, TValue>(comparer);
            dictionaryInterface = concurrentDictionary;
        }

        /// <summary>
        /// Constructs a new fast enumerable concurrent dictionary with the specified collection key value pairs to copy from and the specified comparer used to compare keys
        /// </summary>
        /// <param name="collection">Collection key value pairs to copy from</param>
        /// <param name="comparer">Comparer used to compare keys</param>
        public FastEnumerableConcurrentDictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection, IEqualityComparer<TKey> comparer)
        {
            concurrentDictionary = new ConcurrentDictionary<TKey, TValue>(collection, comparer);
            dictionaryInterface = concurrentDictionary;
        }

        /// <summary>
        /// Constructs a new fast enumerable concurrent dictionary with an estimated number of threads that will update this collection and the initial capacity set
        /// </summary>
        /// <param name="concurrencyLevel">Estimated number of threads that will update this collection</param>
        /// <param name="capacity">Initial capacity of this collection</param>
        public FastEnumerableConcurrentDictionary(int concurrencyLevel, int capacity)
        {
            concurrentDictionary = new ConcurrentDictionary<TKey, TValue>(concurrencyLevel, capacity);
            dictionaryInterface = concurrentDictionary;
        }

        /// <summary>
        /// Constructs a new fast enumerable concurrent dictionary with an estimated number of threads that will update this collection, the specified collection key value pairs to copy from and the specified comparer used to compare keys
        /// </summary>
        /// <param name="concurrencyLevel">Estimated number of threads that will update this collection</param>
        /// <param name="collection">Collection key value pairs to copy from</param>
        /// <param name="comparer">Comparer used to compare keys</param>
        public FastEnumerableConcurrentDictionary(int concurrencyLevel, IEnumerable<KeyValuePair<TKey, TValue>> collection, IEqualityComparer<TKey> comparer)
        {
            concurrentDictionary = new ConcurrentDictionary<TKey, TValue>(concurrencyLevel, collection, comparer);
            dictionaryInterface = concurrentDictionary;
        }

        /// <summary>
        /// Constructs a new fast enumerable concurrent dictionary with an estimated number of threads that will update this collection, the initial capacity set and the specified comparer used to compare keys
        /// </summary>
        /// <param name="concurrencyLevel">Estimated number of threads that will update this collection</param>
        /// <param name="capacity">Initial capacity of this collection</param>
        /// <param name="comparer">Comparer used to compare keys</param>
        public FastEnumerableConcurrentDictionary(int concurrencyLevel, int capacity, IEqualityComparer<TKey> comparer)
        {
            concurrentDictionary = new ConcurrentDictionary<TKey, TValue>(concurrencyLevel, capacity, comparer);
            dictionaryInterface = concurrentDictionary;
        }

        /// <summary>
        /// Adds or updates a value using the specified factories at the specified key
        /// </summary>
        /// <typeparam name="TArg">Factory argument type</typeparam>
        /// <param name="key">Key to add or update the value at</param>
        /// <param name="addValueFactory">Factory responsible for adding a new value</param>
        /// <param name="updateValueFactory">Factory responsible for updating the value</param>
        /// <param name="factoryArgument">Factory argument</param>
        /// <returns>The actual value being added or updated</returns>
        public TValue AddOrUpdate<TArg>
        (
            TKey key,
            Func<TKey, TArg, TValue> addValueFactory,
            Func<TKey, TValue, TArg, TValue> updateValueFactory,
            TArg factoryArgument
        ) =>
            concurrentDictionary.AddOrUpdate
            (
                key,
                (key, factoryArgument) =>
                {
                    areFastEnumerableKeyValuePairsOutdated = true;
                    areFastEnumerableKeysOutdated = true;
                    areFastEnumerableValuesOutdated = true;
                    return addValueFactory(key, factoryArgument);
                },
                (key, value, factoryArgument) =>
                {
                    TValue ret = updateValueFactory(key, value, factoryArgument);
                    if
                    (
                        (
                            !areFastEnumerableKeyValuePairsOutdated ||
                            !areFastEnumerableValuesOutdated
                        ) &&
                        !isValueEqualTo(value, ret))
                    {
                        areFastEnumerableKeyValuePairsOutdated = true;
                        areFastEnumerableValuesOutdated = true;
                    }
                    return ret;
                },
                factoryArgument
            );

        /// <summary>
        /// Adds or updates a value using the specified value or factory at the specified key
        /// </summary>
        /// <param name="key">Key to add or update the value at</param>
        /// <param name="addValue">Value to be added</param>
        /// <param name="updateValueFactory">Factory responsible for updating the value</param>
        /// <returns>The actual value being added or updated</returns>
        public TValue AddOrUpdate(TKey key, TValue addValue, Func<TKey, TValue, TValue> updateValueFactory)
        {
            bool is_value_added = true;
            TValue ret =
                concurrentDictionary.AddOrUpdate
                (
                    key,
                    addValue,
                    (key, value) =>
                    {
                        is_value_added = false;
                        TValue ret = updateValueFactory(key, value);
                        if
                        (
                            (
                                !areFastEnumerableKeyValuePairsOutdated ||
                                !areFastEnumerableValuesOutdated
                            ) &&
                            !isValueEqualTo(value, ret)
                        )
                        {
                            areFastEnumerableKeyValuePairsOutdated = true;
                            areFastEnumerableValuesOutdated = true;
                        }
                        return ret;
                    }
                );
            if (is_value_added)
            {
                areFastEnumerableKeyValuePairsOutdated = true;
                areFastEnumerableKeysOutdated = true;
                areFastEnumerableValuesOutdated = true;
            }
            return ret;
        }

        /// <summary>
        /// Adds or updates a value using factories
        /// </summary>
        /// <param name="key">Key to add or update the value at</param>
        /// <param name="addValueFactory">Factory responsible for adding the value</param>
        /// <param name="updateValueFactory">Factory responsible for updating the value</param>
        /// <returns></returns>
        public TValue AddOrUpdate(TKey key, Func<TKey, TValue> addValueFactory, Func<TKey, TValue, TValue> updateValueFactory) =>
            concurrentDictionary.AddOrUpdate
            (
                key,
                (key) =>
                {
                    areFastEnumerableKeyValuePairsOutdated = true;
                    areFastEnumerableKeysOutdated = true;
                    areFastEnumerableValuesOutdated = true;
                    return addValueFactory(key);
                },
                (key, value) =>
                {
                    TValue ret = updateValueFactory(key, value);
                    if
                    (
                        (
                            !areFastEnumerableKeyValuePairsOutdated ||
                            !areFastEnumerableValuesOutdated
                        ) &&
                        !isValueEqualTo(value, ret)
                    )
                    {
                        areFastEnumerableKeyValuePairsOutdated = true;
                        areFastEnumerableValuesOutdated = true;
                    }
                    return ret;
                }
            );

        /// <summary>
        /// Clears this collection
        /// </summary>
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

        /// <summary>
        /// Is the specified key contained in this collection
        /// </summary>
        /// <param name="key">Key to check if it exists in this collection or not</param>
        /// <returns>"true" if the specified key exists within this collection, otherwise "false"</returns>
        public bool ContainsKey(TKey key) => concurrentDictionary.ContainsKey(key);

        /// <summary>
        /// Gets the enumerator of this collection
        /// </summary>
        /// <returns>Enumerator of this collection</returns>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            if (areFastEnumerableKeyValuePairsOutdated)
            {
                fastEnumerableKeyValuePairs = concurrentDictionary.ToArray();
                areFastEnumerableKeyValuePairsOutdated = false;
            }
            foreach (KeyValuePair<TKey, TValue> key_value_pair in fastEnumerableKeyValuePairs)
            {
                yield return key_value_pair;
            }
        }

        /// <summary>
        /// Gets or adds a value using a factory to this collection
        /// </summary>
        /// <param name="key">Key to get or add a value to this collection</param>
        /// <param name="valueFactory">Factory responsible for adding the value</param>
        /// <returns>Existing value from the specified key or the value added from the specified factory</returns>
        public TValue GetOrAdd(TKey key, Func<TKey, TValue> valueFactory) =>
            concurrentDictionary.GetOrAdd
            (
                key,
                (key) =>
                {
                    areFastEnumerableKeyValuePairsOutdated = true;
                    areFastEnumerableKeysOutdated = true;
                    areFastEnumerableValuesOutdated = true;
                    return valueFactory(key);
                }
            );

        /// <summary>
        /// Gets or adds the specified value to this collection
        /// </summary>
        /// <param name="key">Key to get or add the spcified value to</param>
        /// <param name="value">Value to be added, if it is not contained</param>
        /// <returns>Existing value from the specified key or the specified value added to this collection</returns>
        public TValue GetOrAdd(TKey key, TValue value)
        {
            TValue ret = concurrentDictionary.GetOrAdd(key, value);
            if
            (
                (
                    !areFastEnumerableKeyValuePairsOutdated ||
                    !areFastEnumerableKeysOutdated ||
                    !areFastEnumerableValuesOutdated
                ) &&
                isValueEqualTo(value, ret)
            )
            {
                areFastEnumerableKeyValuePairsOutdated = true;
                areFastEnumerableKeysOutdated = true;
                areFastEnumerableValuesOutdated = true;
            }
            return ret;
        }

        /// <summary>
        /// Gets or adds a value using a factory to this collection
        /// </summary>
        /// <param name="key">Key to get or add a value to this collection</param>
        /// <param name="valueFactory">Factory responsible for adding the value</param>
        /// <param name="factoryArgument">Argument used within the factory</param>
        /// <returns>Existing value from the specified key or the value added from the specified factory</returns>
        public TValue GetOrAdd<TArg>(TKey key, Func<TKey, TArg, TValue> valueFactory, TArg factoryArgument) =>
            concurrentDictionary.GetOrAdd
            (
                key,
                (key, factoryArgument) =>
                {
                    areFastEnumerableKeyValuePairsOutdated = true;
                    areFastEnumerableKeysOutdated = true;
                    areFastEnumerableValuesOutdated = true;
                    return valueFactory(key, factoryArgument);
                },
                factoryArgument
            );

        /// <summary>
        /// Creates a new array from this collection
        /// </summary>
        /// <returns>Newly created array from this collection</returns>
        public KeyValuePair<TKey, TValue>[] ToArray() => concurrentDictionary.ToArray();

        /// <summary>
        /// Tries to add the specified value at the specified key to this collection
        /// </summary>
        /// <param name="key">Key to add the spcifieid value to</param>
        /// <param name="value">Value to be added</param>
        /// <returns>"true" if the specified value has been successfully added to this collection, otherwise "false" if the specified key already exists within this collection</returns>
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

        /// <summary>
        /// Tries to get the value at the specified key
        /// </summary>
        /// <param name="key">Key to get the value from</param>
        /// <param name="value">Value retrieved from this collection</param>
        /// <returns>"true" if the value has been successfully retrieved from this collection, otherwise "false"</returns>
        public bool TryGetValue(TKey key, out TValue value) => concurrentDictionary.TryGetValue(key, out value);

        /// <summary>
        /// Tries to remove the specified key with its associated value within this collection
        /// </summary>
        /// <param name="key">Key to remove the ke value pair from this collection</param>
        /// <param name="value">Value being removed</param>
        /// <returns>"true" if the specified key and associated value has been successfully removed from this collection, otherwise "false"</returns>
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

        /// <summary>
        /// Tries to update with the specified key value pair
        /// </summary>
        /// <param name="key">Key to update the value at</param>
        /// <param name="newValue">Updated value</param>
        /// <param name="comparisonValue">Value compared with the currently stored value to avoid race conditions</param>
        /// <returns></returns>
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

        /// <summary>
        /// Adds the specified value to this collection
        /// </summary>
        /// <param name="key">Key to add the value at</param>
        /// <param name="value">Value to be added</param>
        void IDictionary<TKey, TValue>.Add(TKey key, TValue value)
        {
            dictionaryInterface.Add(key, value);
            areFastEnumerableKeyValuePairsOutdated = true;
            areFastEnumerableKeysOutdated = true;
            areFastEnumerableValuesOutdated = true;
        }

        /// <summary>
        /// Adds the specified key value pair to this collection
        /// </summary>
        /// <param name="item">Key value pair to be added</param>
        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
        {
            dictionaryInterface.Add(item);
            areFastEnumerableKeyValuePairsOutdated = true;
            areFastEnumerableKeysOutdated = true;
            areFastEnumerableValuesOutdated = true;
        }

        /// <summary>
        /// Checks if the specified key value pair is contained within this collection
        /// </summary>
        /// <param name="item">Key value pair to check</param>
        /// <returns>"true" if the specified key value pair is contained within this collection, otherwise "false"</returns>
        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item) => dictionaryInterface.Contains(item);

        /// <summary>
        /// Copies the contents of this collection to the specified array
        /// </summary>
        /// <param name="array">Array to copy key value pairs to</param>
        /// <param name="arrayIndex">Starting array index</param>
        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) => dictionaryInterface.CopyTo(array, arrayIndex);

        /// <summary>
        /// Gets the enumerator for this collection
        /// </summary>
        /// <returns>Enumerator for this collection</returns>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// Removes the specified key with its associated value from this collection
        /// </summary>
        /// <param name="key">Key to remove</param>
        /// <returns>"true" if the specified key and associated value have been successfully removed from this collection, otherwise "false"</returns>
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

        /// <summary>
        /// Removes the specified key value pair from this collection
        /// </summary>
        /// <param name="item">Key value pair to remove</param>
        /// <returns>"true" if the specified key value pair has been successfully removed from this collection, otherwise "false"</returns>
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
