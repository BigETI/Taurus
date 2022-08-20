using Newtonsoft.Json;
using System;
using Taurus.Validators;

/// <summary>
/// Taurus synchronizers data messages namespace
/// </summary>
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
            EnumeratorValidator.IsEnumeratorValid(ErrorType, EErrorType.Invalid) &&
            StringValidator.IsStringNotEmptyOrHasNoWhitespaces(IssuingMessageType) &&
            Validator.IsNotNull(Message);

        /// <summary>
        /// Constructs a new error message for deserializers
        /// </summary>
        public ErrorMessageData()
        {
            // ...
        }

        /// <summary>
        /// Constructs a new error message
        /// </summary>
        /// <param name="errorType">Error type</param>
        /// <param name="errorMessageType">Error message type</param>
        /// <param name="message">Error message</param>
        public ErrorMessageData(EErrorType errorType, string errorMessageType, string message)
        {
            EnumeratorValidator.ValidateEnumerator(errorType, EErrorType.Invalid, nameof(errorType));
            StringValidator.ValidateStringIsNotEmptyOrHasNoWhitespaces(errorMessageType, nameof(errorMessageType));
            ErrorType = errorType;
            IssuingMessageType = errorMessageType;
            Message = message ?? throw new ArgumentNullException(nameof(message));
        }
    }
}
