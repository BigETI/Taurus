using System;
using System.Threading.Tasks;

/// <summary>
/// Taurus synchronizers namespace
/// </summary>
namespace Taurus.Synchronizers
{
    /// <summary>
    /// Used to invoke when an user message validation has failed
    /// </summary>
    /// <typeparam name="TUser">User type</typeparam>
    /// <typeparam name="TSynchronizer">Synchronizer type</typeparam>
    /// <typeparam name="TMessageData">Message data tyoe</typeparam>
    /// <param name="user">Sending user</param>
    /// <param name="message">Message</param>
    /// <param name="bytes">Message bytes</param>
    /// <returns>Task</returns>
    public delegate Task UserMessageValidationFailedDelegate<TUser, TSynchronizer, TMessageData>
    (
        TUser user,
        TMessageData message,
        ReadOnlyMemory<byte> bytes
    )
        where TUser : IUser<TUser, TSynchronizer>
        where TSynchronizer : ISynchronizer<TSynchronizer, TUser>
        where TMessageData : IBaseMessageData;
}
