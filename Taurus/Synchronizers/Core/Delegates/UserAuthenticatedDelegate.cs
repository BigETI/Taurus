/// <summary>
/// Taurus synchronizers namespace
/// </summary>
namespace Taurus.Synchronizers
{
    /// <summary>
    /// Used to invoke when an user has been authenticated
    /// </summary>
    /// <typeparam name="TUser">User type</typeparam>
    /// <param name="user">User</param>
    public delegate void UserAuthenticatedDelegate<TUser>(TUser user) where TUser : IUser;
}
