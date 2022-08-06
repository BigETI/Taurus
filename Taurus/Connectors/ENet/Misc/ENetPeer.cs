using ENet;
using System;

/// <summary>
/// Taurus connectors ENet namespace
/// </summary>
namespace Taurus.Connectors.ENet
{
    /// <summary>
    /// A structure that describes an ENet peer
    /// </summary>
    internal class ENetPeer : APeer, IENetPeer
    {
        /// <summary>
        /// ENet peer
        /// </summary>
        public Peer Peer { get; }

        /// <summary>
        /// Constructs a new ENet peer
        /// </summary>
        /// <param name="guid">Peer GUID</param>
        /// <param name="connector">Connector</param>
        /// <param name="peer">ENet Peer</param>
        public ENetPeer(Guid guid, IConnector connector, Peer peer) : base(guid, connector)
        {
            if (!peer.IsSet)
            {
                throw new ArgumentException("ENet peer is not set.", nameof(peer));
            }
            Peer = peer;
        }
    }
}
