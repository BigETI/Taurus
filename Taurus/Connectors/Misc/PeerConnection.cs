using System.Threading.Tasks;

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

        /// <summary>
        /// COnstructs a new peer connection
        /// </summary>
        /// <param name="peer">Peer</param>
        /// <param name="isPeerConnectionSuccessfulTask">Is connection successful task</param>
        public PeerConnection(IPeer peer, Task<bool> isPeerConnectionSuccessfulTask)
        {
            Peer = peer;
            IsPeerConnectionSuccessfulTask = isPeerConnectionSuccessfulTask;
        }
    }
}
