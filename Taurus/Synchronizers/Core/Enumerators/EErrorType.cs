using Newtonsoft.Json;
using Taurus.JSONConverters;

/// <summary>
/// Taurus synchronizers namespace
/// </summary>
namespace Taurus.Synchronizers
{
    /// <summary>
    /// Error type enumerator
    /// </summary>
    [JsonConverter(typeof(ErrorTypeJSONConverter))]
    public enum EErrorType
    {
        /// <summary>
        /// Invalid error
        /// </summary>
        Invalid,

        /// <summary>
        /// Unknown message
        /// </summary>
        UnknownMessage,

        /// <summary>
        /// Malformed message
        /// </summary>
        MalformedMessage,

        /// <summary>
        /// Not supported message
        /// </summary>
        NotSupportedMessage,

        /// <summary>
        /// Invalid message parameter
        /// </summary>
        InvalidMessageParameters,

        /// <summary>
        /// Message comes from an invalid context
        /// </summary>
        InvalidMessageContext,

        /// <summary>
        /// Received error type is invalid
        /// </summary>
        InvalidErrorType,

        /// <summary>
        /// Received message type is invalid
        /// </summary>
        InvalidMessageType,

        /// <summary>
        /// Received message is invalid
        /// </summary>
        InvalidMessage,

        /// <summary>
        /// Received message is null
        /// </summary>
        MessageIsNull,

        /// <summary>
        /// Unknown reason
        /// </summary>
        Unknown
    }
}
