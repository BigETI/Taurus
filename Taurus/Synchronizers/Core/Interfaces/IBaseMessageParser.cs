using System;
using Taurus.Connectors;
using Taurus.Serializers;

/// <summary>
/// Taurus synchronizers namespace
/// </summary>
namespace Taurus.Synchronizers
{
    /// <summary>
    /// An interface that represents a base message parser
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
