/// <summary>
/// Taurus synchronizers namespace
/// </summary>
namespace Taurus.Synchronizers
{
    /// <summary>
    /// Used to invoke when peer is an user
    /// </summary>
    /// <typeparam name="TUser">User type</typeparam>
    /// <param name="user">User</param>
    public delegate void PeerIsAnUserAssertedDelegate<TUser>(TUser user) where TUser : IUser;
}
