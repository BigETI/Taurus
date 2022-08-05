namespace Taurus.Connectors
{
    /// <summary>
    /// Used to signal a peer being disconnected
    /// </summary>
    /// <param name="peer">Peer</param>
    /// <param name="disconnectionReason">Disconnection reason</param>
    public delegate void PeerDisconnectedDelegate(IPeer peer, EDisconnectionReason disconnectionReason);
}
