/// <summary>
/// Taurus connectors namespace
/// </summary>
namespace Taurus.Connectors
{
    /// <summary>
    /// Used to invoke when a peer has been disconnected
    /// </summary>
    /// <param name="peer">Peer</param>
    /// <param name="disconnectionReason">Disconnection reason</param>
    public delegate void PeerDisconnectedDelegate(IPeer peer, EDisconnectionReason disconnectionReason);
}
