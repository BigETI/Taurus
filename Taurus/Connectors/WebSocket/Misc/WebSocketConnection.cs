using System;

namespace Taurus.Connectors.WebSocket
{
    internal readonly struct WebSocketConnection
    {
        public string Host { get; }

        public ushort Port { get; }

        public WebSocketConnection(string host, ushort port)
        {
            if (string.IsNullOrWhiteSpace(host))
            {
                throw new ArgumentNullException(nameof(host));
            }
            if (port == 0)
            {
                throw new ArgumentException("Port can not be zero.", nameof(port));
            }
            Host = host;
            Port = port;
        }
    }
}
