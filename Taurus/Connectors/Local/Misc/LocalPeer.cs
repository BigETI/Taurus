using System;

/// <summary>
/// Taurus connectors local namespace
/// </summary>
namespace Taurus.Connectors.Local
{
    /// <summary>
    /// A structure that describes a local peer
    /// </summary>
    internal class LocalPeer : APeer, ILocalPeer
    {
        /// <summary>
        /// Target local connector
        /// </summary>
        public ILocalConnector TargetLocalConnector { get; }

        /// <summary>
        /// Constructs a new local peer
        /// </summary>
        /// <param name="guid">Peer GUID</param>
        /// <param name="connector">Connector</param>
        /// <param name="targetLocalConnector">Target local connector</param>
        public LocalPeer(Guid guid, IConnector connector, ILocalConnector targetLocalConnector) : base(guid, connector)
        {
            if (targetLocalConnector == connector)
            {
                throw new ArgumentException("Target local connector can not be the same instance as the specified connector.", nameof(targetLocalConnector));
            }
            TargetLocalConnector = targetLocalConnector;
        }
    }
}
