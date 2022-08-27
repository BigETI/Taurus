using System;
using System.Threading.Tasks;

/// <summary>
/// Taurus synchronizers namespace
/// </summary>
namespace Taurus.Synchronizers
{
    /// <summary>
    /// Used to invoke when parsing an user message has failed
    /// </summary>
    /// <typeparam name="TUser">User type</typeparam>
    /// <typeparam name="TSynchronizer">Synchronizer type</typeparam>
    /// <param name="user">Sending user</param>
    /// <param name="expectedMessageType">Expected message type</param>
    /// <param name="bytes">Message bytes</param>
    /// <returns>Task</returns>
    public delegate Task UserMessageParseFailedDelegate<TUser, TSynchronizer>(TUser user, string expectedMessageType, ReadOnlyMemory<byte> bytes)
        where TUser : IUser<TUser, TSynchronizer>
        where TSynchronizer : ISynchronizer<TSynchronizer, TUser>;
}
