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
        /// Received message is unknown
        /// </summary>
        UnknownMessage,

        /// <summary>
        /// Received message is malformed
        /// </summary>
        MalformedMessage,

        /// <summary>
        /// Received message is not supported
        /// </summary>
        NotSupportedMessage,

        /// <summary>
        /// Specified parameters of a received message are invalid
        /// </summary>
        InvalidMessageParameters,

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
        /// A message has been received in an invalid context
        /// </summary>
        InvalidMessageContext,

        /// <summary>
        /// Message sender is not authenticated
        /// </summary>
        MessageSenderIsNotAuthenticated,

        /// <summary>
        /// Unknown reason
        /// </summary>
        Unknown
    }
}
