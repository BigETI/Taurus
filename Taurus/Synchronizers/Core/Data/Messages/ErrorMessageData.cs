using Newtonsoft.Json;
using System;

namespace Taurus.Synchronizers.Data.Messages
{
    /// <summary>
    /// A class that describes an error message
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    internal class ErrorMessageData : BaseMessageData, IErrorMessageData
    {
        /// <summary>
        /// Error type
        /// </summary>
        [JsonProperty("errorType")]
        public EErrorType ErrorType { get; set; } = EErrorType.Invalid;

        /// <summary>
        /// Message type that caused this error
        /// </summary>
        [JsonProperty("issuingMessageType")]
        public string? IssuingMessageType { get; set; }

        /// <summary>
        /// Error message
        /// </summary>
        [JsonProperty("message")]
        public string? Message { get; set; }

        /// <summary>
        /// Is valid
        /// </summary>
        public override bool IsValid =>
            base.IsValid &&
            (ErrorType != EErrorType.Invalid) &&
            !string.IsNullOrWhiteSpace(IssuingMessageType) &&
            (Message != null);

        /// <summary>
        /// Constructs an error message for deserializers
        /// </summary>
        /// <param name="info">Serialization information</param>
        /// <param name="context">Streaming context</param>
        public ErrorMessageData()
        {
            // ...
        }

        /// <summary>
        /// Constructs an error message
        /// </summary>
        /// <param name="errorType">Error type</param>
        /// <param name="errorMessageType">Error message</param>
        /// <param name="message">Error message</param>
        public ErrorMessageData(EErrorType errorType, string errorMessageType, string message)
        {
            if (errorType == EErrorType.Invalid)
            {
                throw new ArgumentException("Error type can't be invalid.", nameof(errorType));
            }
            if (string.IsNullOrWhiteSpace(errorMessageType))
            {
                throw new ArgumentNullException(nameof(errorMessageType));
            }
            ErrorType = errorType;
            IssuingMessageType = errorMessageType;
            Message = message ?? throw new ArgumentNullException(nameof(message));
        }
    }
}
