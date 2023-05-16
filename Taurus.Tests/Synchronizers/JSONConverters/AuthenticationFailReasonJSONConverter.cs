using Taurus.JSONConverters;

namespace Taurus.Tests.Synchronizers.JSONConverters
{
    internal sealed class AuthenticationFailReasonJSONConverter : AEnumeratorValueJSONConverter<EAuthenticationFailReason>
    {
        public AuthenticationFailReasonJSONConverter() : base(EAuthenticationFailReason.Invalid)
        {
            // ...
        }
    }
}
