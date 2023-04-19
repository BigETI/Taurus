using Taurus.Connectors;
using Taurus.Serializers;
using Taurus.Synchronizers;
using Taurus.Synchronizers.AuthenticableUsers;

namespace Taurus.Tests.Synchronizers
{
    internal sealed class TestAuthenticableServerUser : AAuthenticableUser, ITestAuthenticableServerUser
    {
        public string Username { get; internal set; } = string.Empty;

        string ITestAuthenticableServerUser.Username
        {
            get => Username;
            set => Username = value;
        }

        public TestAuthenticableServerUser(UserGUID userGUID, IPeer peer, ISerializer serializer) : base(userGUID, peer, serializer)
        {
            // ...
        }
    }
}
