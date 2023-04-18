using System.IO.Compression;
using Taurus.Compressors;
using Taurus.Compressors.GZIP;
using Taurus.Connectors.WebSocket;
using Taurus.Fragmenters;
using Taurus.Fragmenters.TaurusFragmenter;
using Taurus.Serializers;
using Taurus.Serializers.BSON;
using Taurus.Serializers.JSON;
using Taurus.Tests.Synchronizers;
using Taurus.Tests.Synchronizers.Data.Messages;

namespace Taurus.Tests
{
    [TestClass]
    public sealed class WebSocketConnectorUnitTests
    {
        private static readonly ushort startPort = 18022;

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

        private static void TestWebSocketMessaging
        (
            ushort port,
            Func<ISerializer> onNewSerializerCreated,
            Func<IFragmenter> onNewFragmenterCreated,
            Func<ICompressor> onNewCompressorCreated
        )
        {
            SharedBoolean isServerNotInitialized = new(true);
            SharedBoolean areThreadsRunning = new(true);
            ushort current_port = port;
            Thread server_thread =
                new
                (
                    () =>
                    {
                        using TestServerSynchronizer test_server_synchronizer = new(onNewSerializerCreated());
                        WebSocketConnector server_web_socket_connector =
                            WebSocketConnector.CreateNewWebSocketConnector
                            (
                                (_) => Task.FromResult(true),
                                onNewFragmenterCreated(),
                                onNewCompressorCreated()
                            );
                        test_server_synchronizer.AddConnector(server_web_socket_connector);
                        test_server_synchronizer.OnTestMessagingCompleted += () => areThreadsRunning.Value = false;
                        server_web_socket_connector.ListenToPort(current_port, 1);
                        isServerNotInitialized.Value = false;
                        ProcessEvents(test_server_synchronizer, areThreadsRunning);
                    }
                );
            Thread client_thread =
                new
                (
                    () =>
                    {
                        Random random = new();
                        using TestClientSynchronizer test_client_synchronizer = new(onNewSerializerCreated());
                        WebSocketConnector client_web_socket_connector =
                            WebSocketConnector.CreateNewWebSocketConnector
                                (
                                    (_) => Task.FromResult(true),
                                    onNewFragmenterCreated(),
                                    onNewCompressorCreated()
                                );
                        test_client_synchronizer.AddConnector(client_web_socket_connector);
                        client_web_socket_connector.ConnectToWebSocket("localhost", current_port);
                        test_client_synchronizer.OnUserConnected +=
                            (user) =>
                            {
                                user.SendMessageAsync(new TestAMessageData(random.Next()));
                            };
                        ProcessEvents(test_client_synchronizer, areThreadsRunning);
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

        [TestMethod]
        public void TestWebSocketMessaging_BSONSerializer_NoFragmententationFragmenter_NoCompressionCompressor()
        {
            TestWebSocketMessaging
            (
                startPort,
                () => new BSONSerializer(),
                () => new NoFragmentationFragmenter(),
                () => new NoCompressionCompressor()
            );
        }

        [TestMethod]
        public void TestWebSocketMessaging_BSONSerializer_NoFragmententationFragmenter_GZIPCompressor()
        {
            TestWebSocketMessaging
            (
                (ushort)(startPort + 1U),
                () => new BSONSerializer(),
                () => new NoFragmentationFragmenter(),
                () => new GZIPCompressor(CompressionLevel.SmallestSize)
            );
        }

        [TestMethod]
        public void TestWebSocketMessaging_BSONSerializer_TaurusFragmenter_NoCompressionCompressor()
        {
            TestWebSocketMessaging
            (
                (ushort)(startPort + 2U),
                () => new BSONSerializer(),
                () => new TaurusFragmenter(),
                () => new NoCompressionCompressor()
            );
        }

        [TestMethod]
        public void TestWebSocketMessaging_BSONSerializer_TaurusFragmenter_GZIPCompressor()
        {
            TestWebSocketMessaging
            (
                (ushort)(startPort + 3U),
                () => new BSONSerializer(),
                () => new TaurusFragmenter(),
                () => new GZIPCompressor(CompressionLevel.SmallestSize)
            );
        }

        [TestMethod]
        public void TestWebSocketMessaging_JSONSerializer_NoFragmententationFragmenter_NoCompressionCompressor()
        {
            TestWebSocketMessaging
            (
                (ushort)(startPort + 4U),
                () => new JSONSerializer(),
                () => new NoFragmentationFragmenter(),
                () => new NoCompressionCompressor()
            );
        }

        [TestMethod]
        public void TestWebSocketMessaging_JSONSerializer_NoFragmententationFragmenter_GZIPCompressor()
        {
            TestWebSocketMessaging
            (
                (ushort)(startPort + 5U),
                () => new JSONSerializer(),
                () => new NoFragmentationFragmenter(),
                () => new GZIPCompressor(CompressionLevel.SmallestSize)
            );
        }

        [TestMethod]
        public void TestWebSocketMessaging_JSONSerializer_TaurusFragmenter_NoCompressionCompressor()
        {
            TestWebSocketMessaging
            (
                (ushort)(startPort + 6U),
                () => new JSONSerializer(),
                () => new TaurusFragmenter(),
                () => new NoCompressionCompressor()
            );
        }

        [TestMethod]
        public void TestWebSocketMessaging_JSONSerializer_TaurusFragmenter_GZIPCompressor()
        {
            TestWebSocketMessaging
            (
                (ushort)(startPort + 7U),
                () => new JSONSerializer(),
                () => new TaurusFragmenter(),
                () => new GZIPCompressor(CompressionLevel.SmallestSize)
            );
        }
    }
}
