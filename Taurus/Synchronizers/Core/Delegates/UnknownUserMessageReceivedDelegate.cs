using System;

/// <summary>
/// Taurus synchronizers namespace
/// </summary>
namespace Taurus.Synchronizers
{
    /// <summary>
    /// Used to invoke when an unknown user message has been received
    /// </summary>
    /// <param name="user">Sending user</param>
    /// <param name="message">Message</param>
    /// <param name="bytes">Message bytes</param>
    public delegate void UnknownUserMessageReceivedDelegate<TUser, TSynchronizer>(TUser user, IBaseMessageData message, ReadOnlySpan<byte> bytes)
        where TUser : IUser<TUser, TSynchronizer>
        where TSynchronizer : ISynchronizer<TSynchronizer, TUser>;
}
