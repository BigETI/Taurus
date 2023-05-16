namespace Taurus.Synchronizers.AuthenticableUsers
{
    /// <summary>
    /// Used to invoke when a peer user authentication has failed
    /// </summary>
    /// <typeparam name="TAuthenticableUser">Authenticable user type</typeparam>
    /// <typeparam name="TAuthenticationFailReason">Authentication fail reason type</typeparam>
    /// <param name="authenticableUser">Authenticable user</param>
    /// <param name="authenticationFailReason">Authentication fail reason</param>
    public delegate void UserAuthenticationFailedDelegate<TAuthenticableUser, TAuthenticationFailReason>
    (
        TAuthenticableUser authenticableUser,
        TAuthenticationFailReason authenticationFailReason
    ) where TAuthenticableUser : IAuthenticableUser;
}
