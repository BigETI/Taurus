using System;
using Taurus.Connectors;

namespace Taurus.Synchronizers
{
    /// <summary>
    /// Used to signal a message being successfully parsed
    /// </summary>
    /// <typeparam name="T">Message type</typeparam>
    /// <param name="peer">Peer</param>
    /// <param name="message">Message</param>
    /// <param name="bytes">Bytes</param>
    public delegate void MessageParsedDelegate<T>(IPeer peer, T message, ReadOnlySpan<byte> bytes) where T : IBaseMessageData;
}
