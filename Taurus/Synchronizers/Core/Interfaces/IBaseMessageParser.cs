using System;
using Taurus.Connectors;
using Taurus.Serializers;

namespace Taurus.Synchronizers
{
    /// <summary>
    /// An interface that represents a base message
    /// </summary>
    public interface IBaseMessageParser
    {
        /// <summary>
        /// Message type
        /// </summary>
        string MessageType { get; }

        /// <summary>
        /// Serializer
        /// </summary>
        ISerializer Serializer { get; }

        /// <summary>
        /// Parses incoming message
        /// </summary>
        /// <param name="peer">Peer</param>
        /// <param name="bytes">Bytes</param>
        void ParseMessage(IPeer peer, ReadOnlySpan<byte> bytes);
    }
}
