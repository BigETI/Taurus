using Newtonsoft.Json;
using Taurus.Synchronizers.Data.Messages;

namespace Taurus.Tests.Synchronizers.Data.Messages
{
    [JsonObject(MemberSerialization.OptIn)]
    internal sealed class AuthenticationSucceededMessageData : BaseMessageData
    {
        [JsonProperty("motd")]
        public string? MOTD { get; set; }

        public override bool IsValid => base.IsValid && (MOTD != null);

        public AuthenticationSucceededMessageData()
        {

        }

        public AuthenticationSucceededMessageData(string motd) => MOTD = motd;
    }
}
