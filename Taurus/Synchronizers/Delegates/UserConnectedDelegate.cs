/// <summary>
/// Taurus synchronizers namespace
/// </summary>
namespace Taurus.Synchronizers
{
    /// <summary>
    /// Used to invoke when an user has been connected
    /// </summary>
    /// <typeparam name="TUser">User type</typeparam>
    /// <param name="user">User</param>
    /// <param name="disconnectionReason">Disconnection reason</param>
    public delegate void UserConnectedDelegate<TUser>(TUser user) where TUser : IUser;
}
