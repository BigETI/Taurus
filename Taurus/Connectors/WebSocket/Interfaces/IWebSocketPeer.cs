using System.Net.Sockets;

namespace Taurus.Connectors.WebSocket
{
    public interface IWebSocketPeer
    {
        /// <summary>
        /// TCP client
        /// </summary>
        TcpClient TCPClient { get; }
    }
}
