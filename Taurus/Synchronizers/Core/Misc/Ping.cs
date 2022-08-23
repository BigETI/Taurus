using System;
using Taurus.Connectors;

/// <summary>
/// Taurus synchronizers namespace
/// </summary>
namespace Taurus.Synchronizers
{
    /// <summary>
    /// A structure that describes a ping
    /// </summary>
    internal readonly struct Ping : IPing
    {
        /// <summary>
        /// Key
        /// </summary>
        public int Key { get; }

        /// <summary>
        /// Peer
        /// </summary>
        public IPeer Peer { get; }

        /// <summary>
        /// Begin date time offset
        /// </summary>
        public DateTimeOffset BeginDateTimeOffset { get; }

        /// <summary>
        /// Constructs a new ping
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="peer">Peer</param>
        /// <param name="beginDateTimeOffset">Begin date time offset</param>
        public Ping(int key, IPeer peer, DateTimeOffset beginDateTimeOffset)
        {
            Key = key;
            Peer = peer;
            BeginDateTimeOffset = beginDateTimeOffset;
        }
    }
}
