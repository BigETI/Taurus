using Taurus.Connectors;

/// <summary>
/// Taurus synchronizers authenticated users namespace
/// </summary>
namespace Taurus.Synchronizers.AuthenticatedUsers
{
    /// <summary>
    /// Used to invoke when a peer user authentication has failed
    /// </summary>
    /// <param name="peer">Peer</param>
    public delegate void PeerUserAuthenticationFailedDelegate(IPeer peer);
}
