using Taurus.Connectors;

/// <summary>
/// Taurus synchronizers namespace
/// </summary>
namespace Taurus.Synchronizers
{
    /// <summary>
    /// Used to invoke when an user has been disconnected
    /// </summary>
    /// <typeparam name="TSynchronizer">Synchronizer type</typeparam>
    /// <typeparam name="TUser">User type</typeparam>
    /// <param name="user">User</param>
    /// <param name="disconnectionReason">Disconnection reason</param>
    public delegate void UserDisconnectedDelegate<TSynchronizer, TUser>(TUser user, EDisconnectionReason disconnectionReason)
        where TSynchronizer : ISynchronizer<TSynchronizer, TUser>
        where TUser : IUser<TUser, TSynchronizer>;
}
