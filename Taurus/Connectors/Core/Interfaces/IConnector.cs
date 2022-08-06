using System;
using System.Collections.Generic;
using Taurus.Compressors;
using Taurus.Fragmenters;

namespace Taurus.Connectors
{
    /// <summary>
    /// An interface that represents any connector
    /// </summary>
    public interface IConnector : IDisposable
    {
        /// <summary>
        /// Connected peers
        /// </summary>
        IReadOnlyDictionary<Guid, IPeer> Peers { get; }

        /// <summary>
        /// Fragmenter
        /// </summary>
        IFragmenter Fragmenter { get; }

        /// <summary>
        /// Compressor
        /// </summary>
        ICompressor Compressor { get; }

        /// <summary>
        /// This event will be invoked when a peer attempted to connect to this connector.
        /// </summary>
        event PeerConnectionAttemptedDelegate? OnPeerConnectionAttempted;

        /// <summary>
        /// This event will be invoked when a peer has been successfully connected to this connector.
        /// </summary>
        event PeerConnectedDelegate? OnPeerConnected;

        /// <summary>
        /// This event will be invoked when a peer has disconnected from this connector.
        /// </summary>
        event PeerDisconnectedDelegate? OnPeerDisconnected;

        /// <summary>
        /// This event will be invoked when a peer has sent a message to this connector
        /// </summary>
        event PeerMessageReceivedDelegate? OnPeerMessageReceived;

        /// <summary>
        /// Is peer contained
        /// </summary>
        /// <param name="peer">Peer</param>
        /// <returns>"true" if peer is contained, otherwise "false"</returns>
        bool IsPeerContained(IPeer peer);

        /// <summary>
        /// Disconnects the specified peer
        /// </summary>
        /// <param name="peer">Peer</param>
        /// <param name="disconnectionReason">Disconnection reason</param>
        void DisconnectPeer(IPeer peer, EDisconnectionReason disconnectionReason);

        /// <summary>
        /// Sends a message to a peer
        /// </summary>
        /// <param name="peer">Peer</param>
        /// <param name="message">Message</param>
        void SendMessageToPeer(IPeer peer, ReadOnlySpan<byte> message);

        /// <summary>
        /// Processes all events appeared since last call
        /// </summary>
        void ProcessEvents();

        /// <summary>
        /// Is connection by peer allowed
        /// </summary>
        /// <param name="peer">Peer</param>
        /// <returns>"true" if connection is allowed, otherwise "false"</returns>
        bool IsConnectionAllowed(IPeer peer);

        /// <summary>
        /// Closes connection to all connected peers in this connector
        /// </summary>
        /// <param name="reason">Disconnection reason</param>
        void Close(EDisconnectionReason reason);
    }
}
