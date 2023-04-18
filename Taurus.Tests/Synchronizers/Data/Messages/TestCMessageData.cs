using Newtonsoft.Json;
using Taurus.Synchronizers.Data.Messages;

namespace Taurus.Tests.Synchronizers.Data.Messages
{
    [JsonObject(MemberSerialization.OptIn)]
    internal sealed class TestCMessageData : BaseMessageData
    {
        [JsonProperty("value")]
        public string? Value { get; set; }

        public TestCMessageData()
        {

        }

        public TestCMessageData(string value) => Value = value;
    }
}
