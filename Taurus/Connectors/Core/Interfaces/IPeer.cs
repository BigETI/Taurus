using System;

namespace Taurus.Connectors
{
    /// <summary>
    /// An interface that represents a peer
    /// </summary>
    public interface IPeer
    {
        /// <summary>
        /// Peer GUID
        /// </summary>
        Guid GUID { get; }

        /// <summary>
        /// Connector
        /// </summary>
        IConnector Connector { get; }

        /// <summary>
        /// Disconnects this peer
        /// </summary>
        /// <param name="peer">Peer</param>
        void Disconnect(EDisconnectionReason disconnectionReason);

        /// <summary>
        /// Sends the specified message to the peer
        /// </summary>
        /// <param name="message">Message</param>
        void SendMessage(ReadOnlySpan<byte> message) => Connector.SendMessageToPeer(this, message);
    }
}
