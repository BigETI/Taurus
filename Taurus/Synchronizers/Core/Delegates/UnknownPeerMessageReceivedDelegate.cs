using System;
using Taurus.Connectors;

/// <summary>
/// Taurus synchronizers namespace
/// </summary>
namespace Taurus.Synchronizers
{
    /// <summary>
    /// Used to invoke when an unknown peer message has been received
    /// </summary>
    /// <param name="peer">Peer</param>
    /// <param name="message">Message</param>
    /// <param name="bytes">Message bytes</param>
    public delegate void UnknownPeerMessageReceivedDelegate(IPeer peer, IBaseMessageData message, ReadOnlySpan<byte> bytes);
}
