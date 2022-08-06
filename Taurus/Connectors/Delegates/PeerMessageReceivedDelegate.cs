using System;

/// <summary>
/// Taurus connectors namespace
/// </summary>
namespace Taurus.Connectors
{
    /// <summary>
    /// Used to invoke when a peer message has been received
    /// </summary>
    /// <param name="peer">Peer</param>
    /// <param name="message">Message</param>
    public delegate void PeerMessageReceivedDelegate(IPeer peer, ReadOnlySpan<byte> message);
}
