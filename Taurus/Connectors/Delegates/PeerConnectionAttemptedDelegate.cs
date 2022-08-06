/// <summary>
/// Taurus connectors namespace
/// </summary>
namespace Taurus.Connectors
{
    /// <summary>
    /// Used to invoke when a peer connection has been attempted
    /// </summary>
    /// <param name="peer">Peer</param>
    public delegate void PeerConnectionAttemptedDelegate(IPeer peer);
}
