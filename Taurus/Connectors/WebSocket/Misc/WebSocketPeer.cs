﻿using System;
using System.Net.Sockets;

namespace Taurus.Connectors.WebSocket
{
    internal class WebSocketPeer : APeer, IWebSocketPeer
    {
        /// <summary>
        /// TCP client
        /// </summary>
        public TcpClient TCPClient { get; }

        /// <summary>
        /// Constructs a new WebSocket peer
        /// </summary>
        /// <param name="guid">GUID</param>
        /// <param name="connector">Connector</param>
        public WebSocketPeer(Guid guid, IConnector connector, TcpClient tcpClient) : base(guid, connector) => TCPClient = tcpClient;
    }
}
