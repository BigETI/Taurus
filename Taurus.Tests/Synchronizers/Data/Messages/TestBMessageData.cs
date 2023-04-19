using Newtonsoft.Json;
using Taurus.Synchronizers.Data.Messages;

namespace Taurus.Tests.Synchronizers.Data.Messages
{
    [JsonObject(MemberSerialization.OptIn)]
    internal sealed class TestBMessageData : BaseMessageData
    {
        [JsonProperty("value")]
        public float Value { get; set; }

        public TestBMessageData()
        {
            // ...
        }

        public TestBMessageData(float value) => Value = value;
    }
}
