using Taurus.Connectors;

/// <summary>
/// Taurus synchronizers authenticated users namespace
/// </summary>
namespace Taurus.Synchronizers.AuthenticatedUsers
{
    /// <summary>
    /// Used to invoke when an user has been disconnected
    /// </summary>
    /// <typeparam name="TUser">User type</typeparam>
    /// <param name="user">User</param>
    /// <param name="disconnectionReason">Disconnection reason</param>
    public delegate void UserDisconnectedDelegate<TUser>(TUser user, EDisconnectionReason disconnectionReason) where TUser : IUser;
}
