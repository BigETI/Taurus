using Newtonsoft.Json;
using Taurus.Synchronizers.Data.Messages;

namespace Taurus.Tests.Synchronizers.Data.Messages
{
    [JsonObject(MemberSerialization.OptIn)]
    internal sealed class AuthenticateMessageData : BaseMessageData
    {
        [JsonProperty("secret")]
        public string? Secret { get; set; }

        public override bool IsValid => base.IsValid && (Secret != null);

        public AuthenticateMessageData()
        {
            // ...
        }

        public AuthenticateMessageData(string secret) => Secret = secret;
    }
}
