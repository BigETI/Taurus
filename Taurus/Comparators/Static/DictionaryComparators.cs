using System.Collections.Generic;

namespace Taurus.Comparators
{
    /// <summary>
    /// A class that contains functionalities to compare dictionaries
    /// </summary>
    public static class DictionaryComparators
    {
        /// <summary>
        /// COmpares the specified dictionaries with each other
        /// </summary>
        /// <typeparam name="TKey">Key type</typeparam>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="leftDictionary">Left dictionary</param>
        /// <param name="rightDictionary">Right dictionary</param>
        /// <returns>"true" if the specified dictionaries are equivalent, otherwise "false"</returns>
        public static bool CompareDictionaries<TKey, TValue>
        (
            IReadOnlyDictionary<TKey, TValue> leftDictionary,
            IReadOnlyDictionary<TKey, TValue> rightDictionary
        )
        {
            bool ret = leftDictionary == rightDictionary;
            if (!ret && (leftDictionary.Count == rightDictionary.Count))
            {
                ret = true;
                foreach (KeyValuePair<TKey, TValue> left_key_value in leftDictionary)
                {
                    if
                    (
                        !rightDictionary.TryGetValue(left_key_value.Key, out TValue right_value) ||
                        !Equals(left_key_value, right_value)
                    )
                    {
                        ret = false;
                        break;
                    }
                }
            }
            return ret;
        }
    }
}
