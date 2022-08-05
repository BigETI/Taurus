using System;

namespace Taurus.Connectors
{
    internal abstract class APeer : IPeer
    {
        /// <summary>
        /// Peer GUID
        /// </summary>
        public Guid GUID { get; }

        /// <summary>
        /// Connector
        /// </summary>
        public IConnector Connector { get; }

        /// <summary>
        /// Constructs a new peer
        /// </summary>
        /// <param name="guid">Peer GUID</param>
        /// <param name="connector">Connector</param>
        /// <exception cref="ArgumentException">When "guid" is empty</exception>
        public APeer(Guid guid, IConnector connector)
        {
            if (guid == Guid.Empty)
            {
                throw new ArgumentException("Peer GUID can not be empty.", nameof(guid));
            }
            GUID = guid;
            Connector = connector;
        }

        /// <summary>
        /// Disconnects this peer
        /// </summary>
        /// <param name="disconnectionReason">Disconnection reason</param>
        public void Disconnect(EDisconnectionReason disconnectionReason) => Connector.DisconnectPeer(this, disconnectionReason);

        /// <summary>
        /// Sends the specified message to the peer
        /// </summary>
        /// <param name="message">Message</param>
        public void SendMessage(ReadOnlySpan<byte> message) => Connector.SendMessageToPeer(this, message);
    }
}
