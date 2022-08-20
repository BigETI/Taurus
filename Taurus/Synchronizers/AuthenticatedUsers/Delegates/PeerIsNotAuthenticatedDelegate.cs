using System.Threading.Tasks;
using Taurus.Connectors;

/// <summary>
/// Taurus synchronizers authenticated users namespace
/// </summary>
namespace Taurus.Synchronizers.AuthenticatedUsers
{
    /// <summary>
    /// Used to invoke when peer is not authenticated
    /// </summary>
    /// <param name="peer">Peer</param>
    /// <returns>Task</returns>
    public delegate Task PeerIsNotAuthenticatedDelegate(IPeer peer);
}
