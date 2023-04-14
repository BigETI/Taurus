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
    internal sealed class ENetPeer : APeer, IENetPeer
    {
        /// <summary>
        /// ENet peer
        /// </summary>
        public Peer Peer { get; }

        /// <summary>
        /// Endpoint
        /// </summary>
        public override string Endpoint => Peer.IP;

        /// <summary>
        /// Constructs a new ENet peer
        /// </summary>
        /// <param name="peerGUID">Peer GUID</param>
        /// <param name="connector">Connector</param>
        /// <param name="peer">ENet Peer</param>
        public ENetPeer(PeerGUID peerGUID, IConnector connector, Peer peer) : base(peerGUID, connector)
        {
            if (!peer.IsSet)
            {
                throw new ArgumentException("ENet peer is not set.", nameof(peer));
            }
            Peer = peer;
        }
    }
}
