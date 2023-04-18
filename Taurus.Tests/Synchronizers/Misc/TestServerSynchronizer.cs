using System.Diagnostics;
using Taurus.Connectors;
using Taurus.Serializers;
using Taurus.Synchronizers;
using Taurus.Tests.Synchronizers.Data.Messages;

namespace Taurus.Tests.Synchronizers
{
    internal sealed class TestServerSynchronizer : ASynchronizer<ITestServerUser>
    {
        public event TestMessagingCompletedDelegate? OnTestMessagingCompleted;

        public TestServerSynchronizer(ISerializer serializer) : base(serializer)
        {
            AddNewAutomaticUserMessageParser<TestAMessageData>
            (
                (user, message, _) =>
                {
                    Debug.WriteLine($"Test A message value: \"{message.Value}\"");
                    return user.SendMessageAsync(new TestBMessageData(message.Value));
                }
            );
            AddNewAutomaticUserMessageParser<TestCMessageData>
            (
                (user, message, _) =>
                {
                    Debug.WriteLine($"Test C message value: \"{message.Value}\"");
                    OnTestMessagingCompleted?.Invoke();
                    return Task.CompletedTask;
                }
            );
        }

        protected override Task<ITestServerUser> CreateNewUserAsync(IPeer peer) =>
            Task.FromResult<ITestServerUser>(new TestServerUser(new UserGUID(Guid.NewGuid()), peer, Serializer));
    }
}
