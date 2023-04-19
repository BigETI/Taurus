using System.Diagnostics;
using Taurus.Connectors;
using Taurus.Serializers.BSON;
using Taurus.Synchronizers;
using Taurus.Synchronizers.AuthenticableUsers;
using Taurus.Tests.Synchronizers.Data.Messages;

namespace Taurus.Tests.Synchronizers
{
    internal sealed class TestAuthenticableUsersServerSynchronizer :
        AAuthenticableUsersSynchronizer
        <
            ITestAuthenticableServerUser,
            AuthenticateMessageData,
            string,
            AuthenticationSucceededMessageData,
            AuthenticationFailedMessageData
        >
    {
        public string Secret { get; }

        public string MOTD { get; }

        public event TestMessagingCompletedDelegate? OnTestMessagingCompleted;

        public TestAuthenticableUsersServerSynchronizer(string secret, string motd) : base(new BSONSerializer())
        {
            Secret = secret;
            MOTD = motd;
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

        protected override Task<ITestAuthenticableServerUser> CreateNewUserAsync(IPeer peer) =>
            Task.FromResult<ITestAuthenticableServerUser>(new TestAuthenticableServerUser(new UserGUID(Guid.NewGuid()), peer, Serializer));

        protected override Task
            <
                IUserAuthenticationResult<string, AuthenticationSucceededMessageData, AuthenticationFailedMessageData>
            > HandlePeerUserAuthenticationAsync(ITestAuthenticableServerUser authenticableUser, AuthenticateMessageData authenticateMessageData)
        {
            return
                Task.FromResult
                (
                    (authenticateMessageData.Secret == Secret) ?
                        UserAuthenticationResult<string, AuthenticationSucceededMessageData, AuthenticationFailedMessageData>.CreateFromSuccess("Test", new(MOTD)) :
                        UserAuthenticationResult<string, AuthenticationSucceededMessageData, AuthenticationFailedMessageData>.CreateFromFailure
                        (
                            new("Invalid secret")
                        )
                );
        }

        protected override void UpdateAuthenticatedUser(ITestAuthenticableServerUser authenticatedUser, string authenticatedUserInformation) =>
            authenticatedUser.Username = authenticatedUserInformation;
    }
}
