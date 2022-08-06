/// <summary>
/// Taurus connectors WebSocket namespace
/// </summary>
namespace Taurus.Connectors.WebSocket
{
    /// <summary>
    /// An interface that represents a WebSocket connector
    /// </summary>
    public interface IWebSocketConnector : IConnector
    {
        /// <summary>
        /// Connects to the specified WebSocket
        /// </summary>
        /// <param name="host">Host</param>
        /// <param name="port">Port</param>
        void ConnectToWebSocket(string host, ushort port);
    }
}
