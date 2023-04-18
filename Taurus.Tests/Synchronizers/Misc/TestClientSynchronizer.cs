using System.Diagnostics;
using Taurus.Connectors;
using Taurus.Serializers;
using Taurus.Synchronizers;
using Taurus.Tests.Synchronizers.Data.Messages;

namespace Taurus.Tests.Synchronizers
{
    internal class TestClientSynchronizer : ASynchronizer<ITestClientUser>
    {
        public TestClientSynchronizer(ISerializer serializer) : base(serializer)
        {
            AddNewAutomaticUserMessageParser<TestBMessageData>
            (
                (user, message, _) =>
                {
                    Debug.WriteLine($"Test B message value: \"{message.Value}\"");
                    return user.SendMessageAsync(new TestCMessageData(message.Value.ToString()));
                }
            );
        }

        protected override Task<ITestClientUser> CreateNewUserAsync(IPeer peer) =>
            Task.FromResult<ITestClientUser>(new TestClientUser(new UserGUID(Guid.NewGuid()), peer, Serializer));
    }
}
