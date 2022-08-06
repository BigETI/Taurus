/// <summary>
/// Taurus connectors namespace
/// </summary>
namespace Taurus.Connectors
{
    /// <summary>
    /// Used to invoke when a peer connection has been denied
    /// </summary>
    /// <param name="peer">Peer</param>
    /// <param name="disconnectionReason">Disconnection reason</param>
    public delegate void PeerConnectionDeniedDelegate(IPeer peer, EDisconnectionReason disconnectionReason);
}
