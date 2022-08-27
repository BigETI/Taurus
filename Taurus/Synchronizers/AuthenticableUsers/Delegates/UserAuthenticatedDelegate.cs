/// <summary>
/// Taurus synchronizers authenticable users namespace
/// </summary>
namespace Taurus.Synchronizers.AuthenticableUsers
{
    /// <summary>
    /// Used to invoke when an user has been authenticated
    /// </summary>
    /// <typeparam name="TAuthenticableUser">Authenticable user type</typeparam>
    /// <typeparam name="TAuthenticableUsersSynchronizer">Authenticable users synchronizer type</typeparam>
    /// <param name="authenticableUser">Authenticable user</param>
    public delegate void UserAuthenticatedDelegate<TAuthenticableUser, TAuthenticableUsersSynchronizer>(TAuthenticableUser authenticableUser)
        where TAuthenticableUser : IAuthenticableUser<TAuthenticableUser, TAuthenticableUsersSynchronizer>
        where TAuthenticableUsersSynchronizer : IAuthenticableUsersSynchronizer<TAuthenticableUsersSynchronizer, TAuthenticableUser>;
}
