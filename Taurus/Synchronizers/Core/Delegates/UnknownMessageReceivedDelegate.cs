using System;

namespace Taurus.Synchronizers
{
    /// <summary>
    /// Used to signal recieving a non-meaningful messge from a peer
    /// </summary>
    /// <param name="message">Message</param>
    /// <param name="bytes">Message bytes</param>
    public delegate void UnknownMessageReceivedDelegate(IBaseMessageData message, ReadOnlySpan<byte> bytes);
}
