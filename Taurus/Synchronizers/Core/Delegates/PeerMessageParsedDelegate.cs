using System;
using System.Threading.Tasks;
using Taurus.Connectors;

/// <summary>
/// Taurus synchronizers namespace
/// </summary>
namespace Taurus.Synchronizers
{
    /// <summary>
    /// Used to invoke when a peer message has been parsed
    /// </summary>
    /// <typeparam name="T">Message type</typeparam>
    /// <param name="peer">Peer</param>
    /// <param name="message">Message</param>
    /// <param name="bytes">Bytes</param>
    /// <returns>Task</returns>
    public delegate Task PeerMessageParsedDelegate<T>(IPeer peer, T message, ReadOnlyMemory<byte> bytes) where T : IBaseMessageData;
}
