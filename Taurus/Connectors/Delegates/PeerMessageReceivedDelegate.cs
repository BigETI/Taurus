using System;

namespace Taurus.Connectors
{
    /// <summary>
    /// Used to signal a message received from a peer
    /// </summary>
    /// <param name="peer">Peer</param>
    /// <param name="message">Message</param>
    public delegate void PeerMessageReceivedDelegate(IPeer peer, ReadOnlySpan<byte> message);
}
