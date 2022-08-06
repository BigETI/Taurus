using System;

/// <summary>
/// Taurus connectors namespace
/// </summary>
namespace Taurus.Connectors
{
    /// <summary>
    /// Used to invoke when a peer message has been sent
    /// </summary>
    /// <param name="peer">Peer</param>
    /// <param name="message">Message</param>
    public delegate void PeerMessageSentDelegate(IPeer peer, ReadOnlySpan<byte> message);
}
