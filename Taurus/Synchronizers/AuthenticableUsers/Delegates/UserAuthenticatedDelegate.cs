/// <summary>
/// Taurus synchronizers authenticable users namespace
/// </summary>
namespace Taurus.Synchronizers.AuthenticableUsers
{
    /// <summary>
    /// Used to invoke when an user has been authenticated
    /// </summary>
    /// <typeparam name="TAuthenticableUser">Authenticable user type</typeparam>
    /// <param name="authenticableUser">Authenticable user</param>
    public delegate void UserAuthenticatedDelegate<TAuthenticableUser>(TAuthenticableUser authenticableUser) where TAuthenticableUser : IAuthenticableUser;
}
