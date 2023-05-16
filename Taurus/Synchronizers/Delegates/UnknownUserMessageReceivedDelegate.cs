using System;

namespace Taurus.Synchronizers
{
    /// <summary>
    /// Used to invoke when an unknown user message has been received
    /// </summary>
    /// <param name="user">Sending user</param>
    /// <param name="message">Message</param>
    /// <param name="bytes">Message bytes</param>
    public delegate void UnknownUserMessageReceivedDelegate<TUser>(TUser user, IBaseMessageData message, ReadOnlySpan<byte> bytes) where TUser : IUser;
}
