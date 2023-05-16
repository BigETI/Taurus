using ENet;

namespace Taurus.Connectors.ENet
{
    /// <summary>
    /// An interface that represents an ENet peer
    /// </summary>
    public interface IENetPeer : IPeer
    {
        /// <summary>
        /// ENet peer
        /// </summary>
        Peer Peer { get; }
    }
}
