using Newtonsoft.Json;
using Taurus.Validators;

/// <summary>
/// Taurus synchronizers data messages namespace
/// </summary>
namespace Taurus.Synchronizers.Data.Messages
{
    /// <summary>
    /// A class that describes the base of any sent or received data
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class BaseMessageData : IBaseMessageData
    {
        /// <summary>
        /// Message type
        /// </summary>
        [JsonProperty("type")]
        public string? MessageType { get; set; }

        /// <summary>
        /// Is object in a valid state
        /// </summary>
        public virtual bool IsValid => StringValidator.IsStringNotEmptyOrHasNoWhitespaces(MessageType);

        /// <summary>
        /// Constructs a base message data with the correct message type
        /// </summary>
        public BaseMessageData() =>
            MessageType = Naming.GetMessageTypeNameFromMessageDataType(GetType());
    }
}
