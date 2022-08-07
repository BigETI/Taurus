using System.Threading.Tasks;

/// <summary>
/// Taurus connectors namespace
/// </summary>
namespace Taurus.Connectors
{
    /// <summary>
    /// A structure that describes a peer connection
    /// </summary>
    internal readonly struct PeerConnection
    {
        /// <summary>
        /// Peer
        /// </summary>
        public IPeer Peer { get; }

        /// <summary>
        /// Is peer connection successful task
        /// </summary>
        public Task<bool> IsPeerConnectionSuccessfulTask { get; }

        public PeerConnection(IPeer peer, Task<bool> isPeerConnectionSuccessfulTask)
        {
            Peer = peer;
            IsPeerConnectionSuccessfulTask = isPeerConnectionSuccessfulTask;
        }
    }
}
