﻿using System;
using System.Threading.Tasks;

namespace Taurus.Connectors
{
    /// <summary>
    /// An interface that represents a peer
    /// </summary>
    public interface IPeer
    {
        /// <summary>
        /// Peer GUID
        /// </summary>
        PeerGUID PeerGUID { get; }

        /// <summary>
        /// Connector
        /// </summary>
        IConnector Connector { get; }

        /// <summary>
        /// Endpoint
        /// </summary>
        string Endpoint { get; }

        /// <summary>
        /// Disconnects this peer
        /// </summary>
        /// <param name="disconnectionReason">Disconnection reason</param>
        void Disconnect(EDisconnectionReason disconnectionReason);

        /// <summary>
        /// Sends the specified message to the peer asynchronously
        /// </summary>
        /// <param name="message">Message</param>
        /// <returns>Task</returns>
        Task SendMessageAsync(ReadOnlyMemory<byte> message);
    }
}
