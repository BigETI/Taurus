/// <summary>
/// Taurus synchronizers authenticable users namespace
/// </summary>
namespace Taurus.Synchronizers.AuthenticableUsers
{
    /// <summary>
    /// Used to invoke when a peer user authentication has failed
    /// </summary>
    /// <typeparam name="TAuthenticableUser">Authenticable user type</typeparam>
    /// <param name="authenticableUser">Authenticable user</param>
    public delegate void UserAuthenticationFailedDelegate<TAuthenticableUser>(TAuthenticableUser authenticableUser)
        where TAuthenticableUser : IAuthenticableUser;
}
