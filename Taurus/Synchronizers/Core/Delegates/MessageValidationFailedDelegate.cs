using System;
using System.Threading.Tasks;
using Taurus.Connectors;

namespace Taurus.Synchronizers
{
    /// <summary>
    /// Used to signal when validation on a message has failed
    /// </summary>
    /// <typeparam name="T">Message tyoe</typeparam>
    /// <param name="peer">Peer</param>
    /// <param name="message">Message</param>
    /// <param name="bytes">Message bytes</param>
    /// <returns>Task</returns>
    public delegate Task MessageValidationFailedDelegate<T>(IPeer peer, T message, ReadOnlyMemory<byte> bytes) where T : IBaseMessageData;
}
