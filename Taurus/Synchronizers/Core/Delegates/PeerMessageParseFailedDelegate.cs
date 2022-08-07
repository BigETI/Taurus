using System;
using System.Threading.Tasks;
using Taurus.Connectors;

/// <summary>
/// Taurus synchronizers namespace
/// </summary>
namespace Taurus.Synchronizers
{
    /// <summary>
    /// Used to invoke when parsing a peer message has failed
    /// </summary>
    /// <param name="peer">Sending peer</param>
    /// <param name="expectedMessageType">Expected message type</param>
    /// <param name="bytes">Message bytes</param>
    /// <returns>Task</returns>
    public delegate Task PeerMessageParseFailedDelegate(IPeer peer, string expectedMessageType, ReadOnlyMemory<byte> bytes);
}
