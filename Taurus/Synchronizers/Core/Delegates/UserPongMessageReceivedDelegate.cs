using System;

/// <summary>
/// Taurus synchronizers namespace
/// </summary>
namespace Taurus.Synchronizers
{
    /// <summary>
    /// Used to invoke when an user ping message has been received
    /// </summary>
    /// <typeparam name="TUser">User type</typeparam>
    /// <typeparam name="TSynchronizer">Synchronizer type</typeparam>
    /// <param name="user">Sending user</param>
    /// <param name="key">Key</param>
    /// <param name="latency">Latency</param>
    public delegate void UserPongMessageReceivedDelegate<TUser, TSynchronizer>(TUser user, int key, TimeSpan latency)
        where TUser : IUser<TUser, TSynchronizer>
        where TSynchronizer : ISynchronizer<TSynchronizer, TUser>;
}
