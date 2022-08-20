using System.Threading.Tasks;
using Taurus.Connectors;

/// <summary>
/// Taurus synchronizers namespace
/// </summary>
namespace Taurus.Synchronizers
{
    /// <summary>
    /// Used to invoke when peer is not authenticated
    /// </summary>
    /// <param name="peer">Peer</param>
    /// <returns>Task</returns>
    public delegate Task PeerIsNotAuthenticatedDelegate(IPeer peer);
}
