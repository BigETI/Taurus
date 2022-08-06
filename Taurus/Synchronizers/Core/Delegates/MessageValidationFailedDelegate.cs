using System;
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
    public delegate void MessageValidationFailedDelegate<T>(IPeer peer, T message, ReadOnlySpan<byte> bytes) where T : IBaseMessageData;
}
