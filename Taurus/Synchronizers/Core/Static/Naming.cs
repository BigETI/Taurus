using System;

namespace Taurus.Synchronizers
{
    /// <summary>
    /// A class used to determine names using naming conventions
    /// </summary>
    public static class Naming
    {
        /// <summary>
        /// Message data suffix
        /// </summary>
        private static readonly string messageDataSuffix = "MessageData";

        /// <summary>
        /// Get message type name from message data type
        /// </summary>
        /// <typeparam name="T">Message type</typeparam>
        /// <returns>Message type</returns>
        public static string GetMessageTypeNameFromMessageDataType<T>() where T : IBaseMessageData =>
            GetMessageTypeNameFromMessageDataType(typeof(T));

        /// <summary>
        /// Get message type name from message data type
        /// </summary>
        /// <param name="type">Type</param>
        /// <returns>Message type</returns>
        public static string GetMessageTypeNameFromMessageDataType(Type type) =>
            type.Name.EndsWith(messageDataSuffix) ? type.Name[..^messageDataSuffix.Length] : type.Name;
    }
}
