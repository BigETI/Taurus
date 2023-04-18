using Taurus.Connectors;
using Taurus.Serializers;
using Taurus.Synchronizers;

namespace Taurus.Tests.Synchronizers
{
    internal sealed class TestServerUser : AUser, ITestServerUser
    {
        public TestServerUser(UserGUID userGUID, IPeer peer, ISerializer serializer) : base(userGUID, peer, serializer)
        {
            // ...
        }
    }
}
