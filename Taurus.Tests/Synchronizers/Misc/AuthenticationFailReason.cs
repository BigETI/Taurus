using Taurus.Validators;

namespace Taurus.Tests.Synchronizers
{
    internal sealed class AuthenticationFailReason
    {
        private string? stringValue;

        public EAuthenticationFailReason Reason { get; }

        public static AuthenticationFailReason InvalidSecret { get; } = new AuthenticationFailReason(EAuthenticationFailReason.InvalidSecret);

        private AuthenticationFailReason(EAuthenticationFailReason reason)
        {
            EnumeratorValidator.ValidateEnumerator(reason, EAuthenticationFailReason.Invalid, nameof(reason));
            Reason = reason;
        }

        public static AuthenticationFailReason GetFromActivityStatus(EAuthenticationFailReason reason) =>
            reason switch
            {
                EAuthenticationFailReason.InvalidSecret => InvalidSecret,
                _ => throw new NotImplementedException(),
            };

        public override bool Equals(object? obj) => (obj is AuthenticationFailReason authentication_fail_reason) && (Reason == authentication_fail_reason.Reason);

        public override int GetHashCode() => Reason.GetHashCode();

        public override string ToString() => stringValue ??= Reason.ToString();

        public static bool operator ==(AuthenticationFailReason? left, AuthenticationFailReason? right) => left?.Reason == right?.Reason;

        public static bool operator !=(AuthenticationFailReason? left, AuthenticationFailReason? right) => left?.Reason != right?.Reason;
    }
}
