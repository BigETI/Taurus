/// <summary>
/// Taurus synchronizers authenticable users namespace
/// </summary>
namespace Taurus.Synchronizers.AuthenticableUsers
{
    /// <summary>
    /// An interface that represents an authenticated user
    /// </summary>
    /// <typeparam name="TAuthenticableUser">Authenticable user type</typeparam>
    /// <typeparam name="TAuthenticableUsersSynchronizer">Authenticable users synchronizer type</typeparam>
    public interface IAuthenticableUser<TAuthenticableUser, TAuthenticableUsersSynchronizer> :
        IUser<TAuthenticableUser, TAuthenticableUsersSynchronizer>
        where TAuthenticableUser : IAuthenticableUser<TAuthenticableUser, TAuthenticableUsersSynchronizer>
        where TAuthenticableUsersSynchronizer : IAuthenticableUsersSynchronizer<TAuthenticableUsersSynchronizer, TAuthenticableUser>
    {
        /// <summary>
        /// Is user authenticated
        /// </summary>
        bool IsAuthenticated { get; }

        /// <summary>
        /// Flags this user as authenticated
        /// </summary>
        internal void FlagAsAuthenticated();
    }
}
