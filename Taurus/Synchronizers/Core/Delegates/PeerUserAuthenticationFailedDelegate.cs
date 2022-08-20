using Taurus.Connectors;

/// <summary>
/// Taurus synchronizers namespace
/// </summary>
namespace Taurus.Synchronizers
{
    /// <summary>
    /// Used to invoke when a peer user authentication has failed
    /// </summary>
    /// <param name="peer">Peer</param>
    public delegate void PeerUserAuthenticationFailedDelegate(IPeer peer);
}
