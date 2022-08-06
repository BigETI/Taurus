using System;

/// <summary>
/// Taurus connectors WebSocket namespace
/// </summary>
namespace Taurus.Connectors.WebSocket
{
    /// <summary>
    /// A structure that describes a WebSOcket connection
    /// </summary>
    internal readonly struct WebSocketConnection
    {
        /// <summary>
        /// Host
        /// </summary>
        public string Host { get; }

        /// <summary>
        /// Port
        /// </summary>
        public ushort Port { get; }

        /// <summary>
        /// Constructs a new WebSocket connection
        /// </summary>
        /// <param name="host">Host</param>
        /// <param name="port">Port</param>
        /// <exception cref="ArgumentNullException">When "host" is null or whitespace</exception>
        /// <exception cref="ArgumentException">When "port" is zero</exception>
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
