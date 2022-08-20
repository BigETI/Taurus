/// <summary>
/// Taurus synchronizers authenticated users namespace
/// </summary>
namespace Taurus.Synchronizers.AuthenticatedUsers
{
    /// <summary>
    /// Used to invoke when peer is an user
    /// </summary>
    /// <param name="user">User</param>
    public delegate void PeerIsAnUserAssertedDelegate<TUser>(IUser user) where TUser : IUser;
}
