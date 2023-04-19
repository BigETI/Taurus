using Newtonsoft.Json;
using Taurus.Synchronizers.Data.Messages;

namespace Taurus.Tests.Synchronizers.Data.Messages
{
    [JsonObject(MemberSerialization.OptIn)]
    internal sealed class AuthenticationFailedMessageData : BaseMessageData
    {
        [JsonProperty("reason")]
        public string? Reason { get; set; }

        public override bool IsValid => base.IsValid && (Reason != null);

        public AuthenticationFailedMessageData()
        {

        }

        public AuthenticationFailedMessageData(string reason) => Reason = reason;
    }
}
