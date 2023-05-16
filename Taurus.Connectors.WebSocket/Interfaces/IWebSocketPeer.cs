using System.Net.Sockets;

namespace Taurus.Connectors.WebSocket
{
    /// <summary>
    /// An interface that represents a WebSocket peer
    /// </summary>
    public interface IWebSocketPeer
    {
        /// <summary>
        /// TCP client
        /// </summary>
        TcpClient TCPClient { get; }
    }
}
