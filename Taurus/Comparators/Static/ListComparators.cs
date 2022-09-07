using System.Collections.Generic;

/// <summary>
/// Taurus comparators namespace
/// </summary>
namespace Taurus.Comparators
{
    /// <summary>
    /// A class that contains functionalities to compare lists
    /// </summary>
    public static class ListComparators
    {
        /// <summary>
        /// Compares the specfied lists with each other
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="leftList">Left list</param>
        /// <param name="rightList">Right list</param>
        /// <returns>"true" if the specified lists are equivalent, otherwise "false"</returns>
        public static bool CompareLists<TValue>(IReadOnlyList<TValue> leftList, IReadOnlyList<TValue> rightList)
        {
            bool ret = leftList == rightList;
            if (!ret && (leftList.Count == rightList.Count))
            {
                ret = true;
                for (int index = 0; index < leftList.Count; index++)
                {
                    TValue left_value = leftList[index];
                    TValue right_value = rightList[index];
                    if (!Equals(left_value, right_value))
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
