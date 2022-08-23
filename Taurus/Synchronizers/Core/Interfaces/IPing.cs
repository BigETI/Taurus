using System;
using Taurus.Connectors;

/// <summary>
/// Taurus synchronizers namespace
/// </summary>
namespace Taurus.Synchronizers
{
    /// <summary>
    /// An interface that represents a ping
    /// </summary>
    public interface IPing
    {
        /// <summary>
        /// Key
        /// </summary>
        int Key { get; }

        /// <summary>
        /// Peer
        /// </summary>
        IPeer Peer { get; }

        /// <summary>
        /// Begin date time offset
        /// </summary>
        DateTimeOffset BeginDateTimeOffset { get; }
    }
}
