using Newtonsoft.Json;

/// <summary>
/// Taurus synchronizers data messages namespace
/// </summary>
namespace Taurus.Synchronizers.Data.Messages
{
    /// <summary>
    /// A class that describes pong message data
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    internal class PongMessageData : BaseMessageData, IPongMessageData
    {
        /// <summary>
        /// Key
        /// </summary>
        [JsonProperty("key")]
        public int? Key { get; set; }

        /// <summary>
        /// Is this object valid
        /// </summary>
        public override bool IsValid => base.IsValid && (Key != null);

        /// <summary>
        /// Constructs a new pong message data
        /// </summary>
        public PongMessageData()
        {
            // ...
        }

        /// <summary>
        /// Constructs a new pong message data
        /// </summary>
        /// <param name="key">Key</param>
        public PongMessageData(int key) => Key = key;
    }
}
