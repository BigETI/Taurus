using System;

namespace Taurus.Connectors.Local
{
    /// <summary>
    /// A structure that describes a local peer
    /// </summary>
    internal sealed class LocalPeer : APeer, ILocalPeer
    {
        /// <summary>
        /// Target local peer
        /// </summary>
        private ILocalPeer? targetLocalPeer;

        /// <summary>
        /// Target local peer
        /// </summary>
        public ILocalPeer? TargetLocalPeer
        {
            get => targetLocalPeer;
            internal set
            {
                if (value == this)
                {
                    throw new ArgumentException("Target local connector can not be of the same instance as this.", nameof(value));
                }
                if (targetLocalPeer != value)
                {
                    if ((targetLocalPeer != null) && (targetLocalPeer.TargetLocalPeer == this))
                    {
                        targetLocalPeer.TargetLocalPeer = null;
                    }
                    targetLocalPeer = value;
                    if ((targetLocalPeer != null) && (targetLocalPeer.TargetLocalPeer == null))
                    {
                        targetLocalPeer.TargetLocalPeer = this;
                    }
                }
            }
        }

        /// <summary>
        /// Target local peer
        /// </summary>
        ILocalPeer? ILocalPeer.TargetLocalPeer
        {
            get => TargetLocalPeer;
            set => TargetLocalPeer = value;
        }

        /// <summary>
        /// Constructs a new local peer
        /// </summary>
        /// <param name="peerGUID">Peer GUID</param>
        /// <param name="connector">Connector</param>
        public LocalPeer(PeerGUID peerGUID, IConnector connector) : base(peerGUID, connector)
        {
            // ...
        }
    }
}
