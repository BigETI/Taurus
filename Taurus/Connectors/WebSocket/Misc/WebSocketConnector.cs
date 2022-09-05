using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Taurus.Compressors;
using Taurus.Fragmenters;
using Taurus.Fragmenters.TaurusFragmenter;

/// <summary>
/// Taurus connectors WebSocket namespace
/// </summary>
namespace Taurus.Connectors.WebSocket
{
    /// <summary>
    /// A class that describes a WebSocket connector
    /// </summary>
    internal class WebSocketConnector : AConnector, IWebSocketConnector
    {
        /// <summary>
        /// Default fragmenter
        /// </summary>
        public static IFragmenter DefaultFragmenter { get; } = new TaurusFragmenter();

        /// <summary>
        /// Connect to host and port events
        /// </summary>
        private readonly ConcurrentQueue<WebSocketConnection> connectToHostAndPortEvents = new ConcurrentQueue<WebSocketConnection>();

        /// <summary>
        /// Connector thread
        /// </summary>
        private readonly Thread connectorThread;

        /// <summary>
        /// Is connector thread running
        /// </summary>
        private volatile bool isConnectorThreadRunning = true;

        /// <summary>
        /// Listening to port
        /// </summary>
        private volatile ushort listeningToPort;

        /// <summary>
        /// Listener backlog
        /// </summary>
        private volatile int listenerBacklog;

        /// <summary>
        /// Buffer
        /// </summary>
        private byte[] buffer = new byte[2048];

        /// <summary>
        /// Constructs a new WebSocket connector
        /// </summary>
        /// <param name="onHandlePeerConnectionAttempt">Gets invoked when a peer connection attempt needs to be handled</param>
        /// <param name="fragmenter">Fragmenter</param>
        /// <param name="compressor">Compressor</param>
        public WebSocketConnector
        (
            HandlePeerConnectionAttemptDelegate onHandlePeerConnectionAttempt,
            IFragmenter? fragmenter = null,
            ICompressor? compressor = null
        ) : base(onHandlePeerConnectionAttempt, fragmenter ?? DefaultFragmenter, compressor)
        {
            connectorThread = new Thread(() =>
            {
                TcpListener? tcp_listener = null;
                ushort listened_to_port = 0;
                int old_listener_backlog = 0;
                while (isConnectorThreadRunning)
                {
                    ushort listening_to_port = listeningToPort;
                    int listener_backlog = listenerBacklog;
                    if (listened_to_port != listening_to_port)
                    {
                        listened_to_port = listening_to_port;
                        old_listener_backlog = listener_backlog;
                        StopTCPListener(tcp_listener);
                        if (listening_to_port != 0)
                        {
                            tcp_listener = new TcpListener(IPAddress.Any, listening_to_port);
                            if (listener_backlog > 0)
                            {
                                tcp_listener.Start(listener_backlog);
                            }
                            else
                            {
                                tcp_listener.Start();
                            }
                        }
                    }
                    else if (old_listener_backlog != listener_backlog)
                    {
                        old_listener_backlog = listener_backlog;
                        StopTCPListener(tcp_listener);
                        if (tcp_listener != null)
                        {
                            if (listener_backlog > 0)
                            {
                                tcp_listener.Start(listener_backlog);
                            }
                            else
                            {
                                tcp_listener.Start();
                            }
                        }
                    }
                    if (tcp_listener != null)
                    {
                        while (tcp_listener.Pending())
                        {
                            EnqueuePeerConnectionAttemptedEvent(new WebSocketPeer(new PeerGUID(Guid.NewGuid()), this, tcp_listener.AcceptTcpClient()));
                        }
                    }
                    ProcessRequests();
                    while (connectToHostAndPortEvents.TryDequeue(out WebSocketConnection connect_to_host_and_port_event))
                    {
                        EnqueuePeerConnectionAttemptedEvent
                        (
                            new WebSocketPeer
                            (
                                new PeerGUID(Guid.NewGuid()),
                                this,
                                new TcpClient(connect_to_host_and_port_event.Host, connect_to_host_and_port_event.Port)
                            )
                        );
                    }
                    foreach (IPeer peer in Peers.Values)
                    {
                        if (peer is IWebSocketPeer web_socket_peer)
                        {
                            int available_bytes_count = web_socket_peer.TCPClient.Available;
                            if (available_bytes_count > 0)
                            {
                                if (buffer.Length < available_bytes_count)
                                {
                                    buffer =
                                        new byte
                                        [
                                            available_bytes_count / buffer.Length * (((available_bytes_count % buffer.Length) == 0) ? 1 : 2) * buffer.Length
                                        ];
                                }
                                int read_bytes_count = web_socket_peer.TCPClient.GetStream().Read(buffer, 0, available_bytes_count);
                                if (read_bytes_count > 0)
                                {
                                    EnqueuePeerMessageReceivedEvent(peer, buffer.AsSpan(0, read_bytes_count));
                                }
                            }
                        }
                    }
                }
                StopTCPListener(tcp_listener);
                ProcessRequests();
            });
            connectorThread.Start();
        }

        /// <summary>
        /// Tries to get a WebSocket peer
        /// </summary>
        /// <param name="peer">Peer</param>
        /// <param name="webSocketPeer">WebSocket peer</param>
        /// <returns></returns>
        private bool TryGettingWebSocketPeer(IPeer peer, out IWebSocketPeer? webSocketPeer)
        {
            bool ret = false;
            if ((peer is IWebSocketPeer web_socket_peer) && Peers.TryGetValue(peer.PeerGUID, out IPeer current_peer) && (peer == current_peer))
            {
                ret = true;
                webSocketPeer = web_socket_peer;
            }
            else
            {
                webSocketPeer = null;
            }
            return ret;
        }

        /// <summary>
        /// Asserts that peer is a WebSocket peer
        /// </summary>
        /// <param name="peer">Peer</param>
        /// <param name="onPeerIsWebSocketPeerAsserted">Gets invoked when peer is a WebSocket peer asserted</param>
        private void AssertPeerIsWebSocketPeer(IPeer peer, PeerIsWebSocketPeerAssertedDelegate onPeerIsWebSocketPeerAsserted)
        {
            if (TryGettingWebSocketPeer(peer, out IWebSocketPeer? web_socket_peer))
            {
                onPeerIsWebSocketPeerAsserted(web_socket_peer!);
            }
        }

        /// <summary>
        /// Stops TCP listener
        /// </summary>
        /// <param name="tcpListener">TCP listener</param>
        private void StopTCPListener(TcpListener? tcpListener)
        {
            tcpListener?.Stop();
            foreach (IPeer peer in Peers.Values)
            {
                peer.Disconnect(EDisconnectionReason.Disposed);
            }
        }

        /// <summary>
        /// Handles peer disconnection request
        /// </summary>
        /// <param name="peer">Peer</param>
        /// <param name="disconnectionReason">Disconnection reason</param>
        protected override void HandlePeerDisconnectionRequest(IPeer peer, EDisconnectionReason disconnectionReason)
        {
            AssertPeerIsWebSocketPeer
            (
                peer,
                (web_socket_peer) =>
                {
                    web_socket_peer.TCPClient.Close();
                    web_socket_peer.TCPClient.Dispose();
                }
            );
        }

        /// <summary>
        /// Handles sending peer message request
        /// </summary>
        /// <param name="peer">Peer</param>
        /// <param name="message">Message</param>
        protected override void HandleSendingPeerMessageRequest(IPeer peer, ReadOnlySpan<byte> message)
        {
            if (TryGettingWebSocketPeer(peer, out IWebSocketPeer? web_socket_peer))
            {
                web_socket_peer!.TCPClient.GetStream().Write(message);
            }
        }

        /// <summary>
        /// Listens to the specified port
        /// </summary>
        /// <param name="port">Port</param>
        /// <param name="maximalPendingConnectionCount">Maximal pending connection count</param>
        /// <exception cref="ArgumentException">When "port" is zero</exception>
        public void ListenToPort(ushort port, int maximalPendingConnectionCount)
        {
            if (port == 0)
            {
                throw new ArgumentException("Listening port can not be zero.", nameof(port));
            }
            listeningToPort = port;
            listenerBacklog = maximalPendingConnectionCount;
        }

        /// <summary>
        /// Stops listening to port
        /// </summary>
        public void StopListening() =>
            listeningToPort = 0;

        /// <summary>
        /// Connects to the specified WebSocket
        /// </summary>
        /// <param name="host">Host</param>
        /// <param name="port">Port</param>
        public void ConnectToWebSocket(string host, ushort port) =>
            connectToHostAndPortEvents.Enqueue(new WebSocketConnection(host, port));

        /// <summary>
        /// Closes connection to all connected peers in this connector
        /// </summary>
        /// <param name="reason">Disconnection reason</param>
        public override void Close(EDisconnectionReason reason)
        {
            base.Close(reason);
            isConnectorThreadRunning = false;
            connectorThread.Join();
        }
    }
}
