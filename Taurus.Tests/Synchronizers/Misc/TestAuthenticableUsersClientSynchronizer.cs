using System.Diagnostics;
using Taurus.Connectors;
using Taurus.Serializers.BSON;
using Taurus.Synchronizers;
using Taurus.Tests.Synchronizers.Data.Messages;

namespace Taurus.Tests.Synchronizers
{
    internal sealed class TestAuthenticableUsersClientSynchronizer : ASynchronizer<ITestAuthenticableClientUser>
    {
        public TestAuthenticableUsersClientSynchronizer() : base(new BSONSerializer())
        {
            AddNewAutomaticUserMessageParser<AuthenticationSucceededMessageData>
            (
                (user, message, _) =>
                {
                    Debug.WriteLine($"MOTD: \"{message.MOTD}\"");
                    return user.SendMessageAsync(new TestAMessageData(new Random().Next()));
                }
            );
            AddNewAutomaticUserMessageParser<TestBMessageData>
            (
                (user, message, _) =>
                {
                    Debug.WriteLine($"Test B message value: \"{message.Value}\"");
                    return user.SendMessageAsync(new TestCMessageData(message.Value.ToString()));
                }
            );
        }

        protected override Task<ITestAuthenticableClientUser> CreateNewUserAsync(IPeer peer) =>
            Task.FromResult<ITestAuthenticableClientUser>(new TestAuthenticableClientUser(new UserGUID(Guid.NewGuid()), peer, Serializer));
    }
}
