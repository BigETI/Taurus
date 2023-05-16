using System;

namespace Taurus.Connectors
{
    /// <summary>
    /// A structure that describes a peer message
    /// </summary>
    internal readonly struct PeerMessage
    {
        /// <summary>
        /// Peer
        /// </summary>
        public IPeer Peer { get; }

        /// <summary>
        /// Message
        /// </summary>
        public ReadOnlyMemory<byte> Message { get; }

        /// <summary>
        /// Constructs a new peer message
        /// </summary>
        /// <param name="peer">Peer</param>
        /// <param name="message">Message</param>
        public PeerMessage(IPeer peer, ReadOnlySpan<byte> message)
        {
            Peer = peer;
            Message = message.ToArray();
        }
    }
}
