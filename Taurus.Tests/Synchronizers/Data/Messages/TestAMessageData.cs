using Newtonsoft.Json;
using Taurus.Synchronizers.Data.Messages;

namespace Taurus.Tests.Synchronizers.Data.Messages
{
    [JsonObject(MemberSerialization.OptIn)]
    internal sealed class TestAMessageData : BaseMessageData
    {
        [JsonProperty("value")]
        public int Value { get; set; }

        public TestAMessageData()
        {

        }

        public TestAMessageData(int value) => Value = value;
    }
}
