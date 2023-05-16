using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Taurus.Compressors;
using Taurus.Fragmenters;

namespace Taurus.Connectors
{
    /// <summary>
    /// An interface that represents any connector
    /// </summary>
    public interface IConnector : IProcessableEvents, IDisposable
    {
        /// <summary>
        /// Peers
        /// </summary>
        IReadOnlyDictionary<PeerGUID, IPeer> Peers { get; }

        /// <summary>
        /// Fragmenter
        /// </summary>
        IFragmenter Fragmenter { get; }

        /// <summary>
        /// Compressor
        /// </summary>
        ICompressor Compressor { get; }

        /// <summary>
        /// Gets invoked when a peer connection has been attempted
        /// </summary>
        event PeerConnectionAttemptedDelegate? OnPeerConnectionAttempted;

        /// <summary>
        /// Gets invoked when a peer has been connected
        /// </summary>
        event PeerConnectedDelegate? OnPeerConnected;

        /// <summary>
        /// Gets invoked when a peer connection has been denied
        /// </summary>
        event PeerConnectionDeniedDelegate? OnPeerConnectionDenied;

        /// <summary>
        /// Gets invoked when a peer has been disconnected
        /// </summary>
        event PeerDisconnectedDelegate? OnPeerDisconnected;

        /// <summary>
        /// Gets invoke when a peer message has been sent
        /// </summary>
        event PeerMessageSentDelegate? OnPeerMessageSent;

        /// <summary>
        /// Gets invoked when a peer message has been received
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
        /// Sends a message to a peer asynchronously
        /// </summary>
        /// <param name="peer">Peer</param>
        /// <param name="message">Message</param>
        /// <returns>Task</returns>
        Task SendMessageToPeerAsync(IPeer peer, ReadOnlyMemory<byte> message);

        /// <summary>
        /// Closes connection to all connected peers in this connector
        /// </summary>
        /// <param name="reason">Disconnection reason</param>
        void Close(EDisconnectionReason reason);
    }
}
