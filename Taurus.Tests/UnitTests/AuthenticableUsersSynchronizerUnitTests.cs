using Taurus.Connectors.Local;
using Taurus.Tests.Synchronizers;
using Taurus.Tests.Synchronizers.Data.Messages;

namespace Taurus.Tests
{
    [TestClass]
    public sealed class AuthenticableUsersSynchronizerUnitTests
    {
        private static readonly double maximalServerInitializationTimeSeconds = 3.0;

        private static readonly double maximalMessagingTimeSeconds = 10.0;

        private static void ProcessEvents(IProcessableEvents processableEvents, SharedBoolean isRunning)
        {
            while (isRunning)
            {
                processableEvents.ProcessEvents();
                Thread.Sleep(20);
            }
        }

        [TestMethod]
        public void TestAuthenticableUsersMessaging()
        {
            SharedBoolean isServerNotInitialized = new(true);
            SharedBoolean areThreadsRunning = new(true);
            LocalConnector server_local_connector = LocalConnector.CreateNewLocalConnector((_) => Task.FromResult(true));
            LocalConnector client_local_connector = LocalConnector.CreateNewLocalConnector((_) => Task.FromResult(true));
            string secret = new Random().Next().ToString();
            Thread server_thread =
                new
                (
                    () =>
                    {
                        using TestAuthenticableUsersServerSynchronizer test_authenticable_users_server_synchronizer = new(secret, "This is a test!");
                        test_authenticable_users_server_synchronizer.AddConnector(server_local_connector);
                        test_authenticable_users_server_synchronizer.OnTestMessagingCompleted += () => areThreadsRunning.Value = false;
                        isServerNotInitialized.Value = false;
                        ProcessEvents(test_authenticable_users_server_synchronizer, areThreadsRunning);
                    }
                );
            Thread client_thread =
                new
                (
                    () =>
                    {
                        using TestAuthenticableUsersClientSynchronizer test_authenticable_users_client_synchronizer = new();
                        test_authenticable_users_client_synchronizer.AddConnector(client_local_connector);
                        test_authenticable_users_client_synchronizer.OnUserConnected +=
                            (user) =>
                            {
                                user.SendMessageAsync(new AuthenticateMessageData(secret));
                            };
                        client_local_connector.ConnectToLocalConnector(server_local_connector);
                        ProcessEvents(test_authenticable_users_client_synchronizer, areThreadsRunning);
                    }
                );
            server_thread.Start();
            DateTimeOffset server_start_date_time_offset = DateTimeOffset.Now;
            while (isServerNotInitialized)
            {
                if ((DateTimeOffset.Now - server_start_date_time_offset).TotalSeconds >= maximalServerInitializationTimeSeconds)
                {
                    areThreadsRunning.Value = false;
                    server_thread.Join();
                    Assert.Fail($"Server could not initialize in time. Given time: {maximalServerInitializationTimeSeconds} seconds");
                    return;
                }
                Thread.Sleep(20);
            }
            client_thread.Start();
            DateTimeOffset client_start_date_time_offset = DateTimeOffset.Now;
            while (areThreadsRunning)
            {
                if ((DateTimeOffset.Now - client_start_date_time_offset).TotalSeconds >= maximalMessagingTimeSeconds)
                {
                    areThreadsRunning.Value = false;
                    server_thread.Join();
                    client_thread.Join();
                    Assert.Fail($"Messaging between client and server failed. Given time: {maximalMessagingTimeSeconds} seconds");
                    return;
                }
                Thread.Sleep(20);
            }
            server_thread.Join();
            client_thread.Join();
        }
    }
}
