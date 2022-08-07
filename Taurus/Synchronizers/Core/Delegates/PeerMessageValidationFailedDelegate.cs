using System;
using System.Threading.Tasks;
using Taurus.Connectors;

/// <summary>
/// Taurus synchronizers namespace
/// </summary>
namespace Taurus.Synchronizers
{
    /// <summary>
    /// Used to invoke when a peer message validation has failed
    /// </summary>
    /// <typeparam name="T">Message tyoe</typeparam>
    /// <param name="peer">Peer</param>
    /// <param name="message">Message</param>
    /// <param name="bytes">Message bytes</param>
    /// <returns>Task</returns>
    public delegate Task PeerMessageValidationFailedDelegate<T>(IPeer peer, T message, ReadOnlyMemory<byte> bytes) where T : IBaseMessageData;
}
