using System.Net.Sockets;

/// <summary>
/// Taurus connectors WebSocket namespace
/// </summary>
namespace Taurus.Connectors.WebSocket
{
    /// <summary>
    /// A class that describes a WebSocket peer
    /// </summary>
    internal sealed class WebSocketPeer : APeer, IWebSocketPeer
    {
        /// <summary>
        /// TCP client
        /// </summary>
        public TcpClient TCPClient { get; }

        /// <summary>
        /// Constructs a new WebSocket peer
        /// </summary>
        /// <param name="peerGUID">Peer GUID</param>
        /// <param name="connector">Connector</param>
        public WebSocketPeer(PeerGUID peerGUID, IConnector connector, TcpClient tcpClient) : base(peerGUID, connector) => TCPClient = tcpClient;
    }
}
