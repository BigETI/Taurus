namespace Taurus.Synchronizers.AuthenticableUsers
{
    /// <summary>
    /// Used to invoke when an user has been authenticated
    /// </summary>
    /// <typeparam name="TAuthenticableUser">Authenticable user type</typeparam>
    /// <typeparam name="TAuthenticatedUserInformation">Authenticated user information</typeparam>
    /// <param name="authenticableUser">Authenticable user</param>
    /// <param name="authenticatedUserInformation">Authenticated user information</param>
    public delegate void UserAuthenticatedDelegate<TAuthenticableUser, TAuthenticatedUserInformation>
    (
        TAuthenticableUser authenticableUser,
        TAuthenticatedUserInformation authenticatedUserInformation
    ) where TAuthenticableUser : IAuthenticableUser;
}
