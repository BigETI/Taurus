using System;

/// <summary>
/// Taurus connectors local namespace
/// </summary>
namespace Taurus.Connectors.Local
{
    /// <summary>
    /// A structure that describes a local peer
    /// </summary>
    internal sealed class LocalPeer : APeer, ILocalPeer
    {
        /// <summary>
        /// Target local connector
        /// </summary>
        public ILocalConnector TargetLocalConnector { get; }

        /// <summary>
        /// Constructs a new local peer
        /// </summary>
        /// <param name="peerGUID">Peer GUID</param>
        /// <param name="connector">Connector</param>
        /// <param name="targetLocalConnector">Target local connector</param>
        public LocalPeer(PeerGUID peerGUID, IConnector connector, ILocalConnector targetLocalConnector) : base(peerGUID, connector)
        {
            if (targetLocalConnector == connector)
            {
                throw new ArgumentException("Target local connector can not be the same instance as the specified connector.", nameof(targetLocalConnector));
            }
            TargetLocalConnector = targetLocalConnector;
        }
    }
}
