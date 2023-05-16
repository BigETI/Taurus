using Newtonsoft.Json;
using Taurus.Tests.Synchronizers.JSONConverters;

namespace Taurus.Tests.Synchronizers
{
    [JsonConverter(typeof(AuthenticationFailReasonJSONConverter))]
    internal enum EAuthenticationFailReason
    {
        Invalid,

        InvalidSecret
    }
}
