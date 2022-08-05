namespace Taurus.Connectors.WebSocket
{
    public interface IWebSocketConnector : IConnector
    {
        /// <summary>
        /// Connects to a WebSocket
        /// </summary>
        /// <param name="Host">Host</param>
        /// <param name="port">Port</param>
        void ConnectToWebSocket(string host, ushort port);
    }
}
