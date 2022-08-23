using System;
using Taurus.Connectors;

/// <summary>
/// Taurus synchronizers namespace
/// </summary>
namespace Taurus.Synchronizers
{
    /// <summary>
    /// Used to invoke when a peer ping message has been received
    /// </summary>
    /// <param name="peer">Peer</param>
    /// <param name="key">Key</param>
    /// <param name="latency">Latency</param>
    public delegate void PeerPongMessageReceivedDelegate(IPeer peer, int key, TimeSpan latency);
}
