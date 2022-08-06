/// <summary>
/// Taurus connectors namespace
/// </summary>
namespace Taurus.Connectors
{
    /// <summary>
    /// A structure that describes a peer disconnection
    /// </summary>
    internal readonly struct PeerDisconnection
    {
        /// <summary>
        /// Peer
        /// </summary>
        public IPeer Peer { get; }

        /// <summary>
        /// Disconnection reason
        /// </summary>
        public EDisconnectionReason DisconnectionReason { get; }

        /// <summary>
        /// Constructs a new peer disconnection
        /// </summary>
        /// <param name="peer">Peer</param>
        /// <param name="disconnectionReason">Disconnection reason</param>
        public PeerDisconnection(IPeer peer, EDisconnectionReason disconnectionReason)
        {
            Peer = peer;
            DisconnectionReason = disconnectionReason;
        }
    }
}
