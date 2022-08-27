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
    public delegate void UserPingMessageReceivedDelegate<TUser, TSynchronizer>(TUser user, int key)
        where TUser : IUser<TUser, TSynchronizer>
        where TSynchronizer : ISynchronizer<TSynchronizer, TUser>;
}
