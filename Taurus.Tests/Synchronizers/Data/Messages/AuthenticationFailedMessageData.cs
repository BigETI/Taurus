using Newtonsoft.Json;
using Taurus.Synchronizers.Data.Messages;
using Taurus.Validators;

namespace Taurus.Tests.Synchronizers.Data.Messages
{
    [JsonObject(MemberSerialization.OptIn)]
    internal sealed class AuthenticationFailedMessageData : BaseMessageData
    {
        [JsonProperty("reason")]
        public EAuthenticationFailReason Reason { get; set; }

        public override bool IsValid => base.IsValid && EnumeratorValidator.IsEnumeratorValid(Reason, EAuthenticationFailReason.Invalid);

        public AuthenticationFailedMessageData()
        {

        }

        public AuthenticationFailedMessageData(EAuthenticationFailReason reason)
        {
            EnumeratorValidator.ValidateEnumerator(reason, EAuthenticationFailReason.Invalid, nameof(reason));
            Reason = reason;
        }
    }
}
