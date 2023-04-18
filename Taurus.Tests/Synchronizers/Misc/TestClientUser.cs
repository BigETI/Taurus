using Taurus.Connectors;
using Taurus.Serializers;
using Taurus.Synchronizers;

namespace Taurus.Tests.Synchronizers
{
    internal sealed class TestClientUser : AUser, ITestClientUser
    {
        public TestClientUser(UserGUID userGUID, IPeer peer, ISerializer serializer) : base(userGUID, peer, serializer)
        {
            // ...
        }
    }
}
