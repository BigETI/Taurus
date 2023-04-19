using Taurus.Connectors;
using Taurus.Serializers;
using Taurus.Synchronizers;
using Taurus.Synchronizers.AuthenticableUsers;

namespace Taurus.Tests.Synchronizers
{
    internal sealed class TestAuthenticableClientUser : AAuthenticableUser, ITestAuthenticableClientUser
    {
        public string Username { get; internal set; } = string.Empty;

        string ITestAuthenticableClientUser.Username
        {
            get => Username;
            set => Username = value;
        }

        public TestAuthenticableClientUser(UserGUID userGUID, IPeer peer, ISerializer serializer) : base(userGUID, peer, serializer)
        {
            // ...
        }
    }
}
