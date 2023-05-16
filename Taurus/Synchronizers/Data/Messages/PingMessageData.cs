using Newtonsoft.Json;

namespace Taurus.Synchronizers.Data.Messages
{
    /// <summary>
    /// A class that describes ping message data
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    internal class PingMessageData : BaseMessageData, IPingMessageData
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
        /// Constructs a new ping message data
        /// </summary>
        public PingMessageData()
        {
            // ...
        }

        /// <summary>
        /// Constructs a new ping message data
        /// </summary>
        /// <param name="key">Key</param>
        public PingMessageData(int key) => Key = key;
    }
}
