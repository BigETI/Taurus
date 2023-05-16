using System.Net;
using System.Net.Sockets;

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
        /// Endpoint
        /// </summary>
        public override string Endpoint => (TCPClient.Client.RemoteEndPoint is IPEndPoint ip_endpoint) ? ip_endpoint.Address.ToString() : string.Empty;

        /// <summary>
        /// Constructs a new WebSocket peer
        /// </summary>
        /// <param name="peerGUID">Peer GUID</param>
        /// <param name="connector">Connector</param>
        /// <param name="tcpClient">TCP client</param>
        public WebSocketPeer(PeerGUID peerGUID, IConnector connector, TcpClient tcpClient) : base(peerGUID, connector) => TCPClient = tcpClient;
    }
}
