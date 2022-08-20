/// <summary>
/// Taurus synchronizers authenticated users namespace
/// </summary>
namespace Taurus.Synchronizers.AuthenticatedUsers
{
    /// <summary>
    /// Used to invoke when an user has been authenticated
    /// </summary>
    /// <typeparam name="TUser">User type</typeparam>
    /// <param name="user">User</param>
    public delegate void UserAuthenticatedDelegate<TUser>(TUser user) where TUser : IUser;
}
