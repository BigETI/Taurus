using System;

namespace Taurus.Synchronizers
{
    /// <summary>
    /// Used to signal recieving a non-meaningful messge from a peer
    /// </summary>
    /// <param name="message">Message</param>
    /// <param name="bson">Message BSON</param>
    public delegate void UnknownMessageReceivedDelegate(IBaseMessageData message, ReadOnlySpan<byte> bson);
}
