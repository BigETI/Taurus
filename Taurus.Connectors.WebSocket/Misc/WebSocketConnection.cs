using System;
using Taurus.Validators;

namespace Taurus.Connectors.WebSocket
{
    /// <summary>
    /// A structure that describes a WebSocket connection
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
            StringValidator.ValidateStringIsNotEmptyOrHasNoWhitespaces(host, nameof(host));
            Validator.Validate(port, nameof(port), (input) => input > 0);
            Host = host;
            Port = port;
        }
    }
}
