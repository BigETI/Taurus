using System;

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
        /// Listens to the specified port
        /// </summary>
        /// <param name="port">Port</param>
        /// <param name="maximalPendingConnectionCount">Maximal pending connection count</param>
        /// <exception cref="ArgumentException">When "port" is zero</exception>
        void ListenToPort(ushort port, int maximalPendingConnectionCount);

        /// <summary>
        /// Stops listening to port
        /// </summary>
        void StopListening();

        /// <summary>
        /// Connects to the specified WebSocket
        /// </summary>
        /// <param name="host">Host</param>
        /// <param name="port">Port</param>
        void ConnectToWebSocket(string host, ushort port);
    }
}
