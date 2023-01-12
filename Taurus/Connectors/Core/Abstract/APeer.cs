using System;
using System.Threading.Tasks;
using Taurus.Validators;

/// <summary>
/// Taurus connectors namespace
/// </summary>
namespace Taurus.Connectors
{
    /// <summary>
    /// An abstract class that describes a peer
    /// </summary>
    internal abstract class APeer : IPeer
    {
        /// <summary>
        /// Peer GUID
        /// </summary>
        public PeerGUID PeerGUID { get; }

        /// <summary>
        /// Connector
        /// </summary>
        public IConnector Connector { get; }

        /// <summary>
        /// Endpoint
        /// </summary>
        public virtual string Endpoint => string.Empty;

        /// <summary>
        /// Constructs a new peer
        /// </summary>
        /// <param name="peerGUID">Peer GUID</param>
        /// <param name="connector">Connector</param>
        /// <exception cref="ArgumentException">When "guid" is empty</exception>
        protected APeer(PeerGUID peerGUID, IConnector connector)
        {
            GUIDValidator.ValidateGUID(peerGUID, nameof(peerGUID));
            PeerGUID = peerGUID;
            Connector = connector;
        }

        /// <summary>
        /// Disconnects this peer
        /// </summary>
        /// <param name="disconnectionReason">Disconnection reason</param>
        public void Disconnect(EDisconnectionReason disconnectionReason) => Connector.DisconnectPeer(this, disconnectionReason);

        /// <summary>
        /// Sends the specified message to the peer asynchronously
        /// </summary>
        /// <param name="message">Message</param>
        /// <returns>Task</returns>
        public Task SendMessageAsync(ReadOnlyMemory<byte> message) => Connector.SendMessageToPeerAsync(this, message);
    }
}
