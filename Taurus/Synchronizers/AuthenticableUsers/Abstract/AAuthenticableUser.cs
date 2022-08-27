using Taurus.Connectors;

/// <summary>
/// Taurus synchronizers authenticable users namespace
/// </summary>
namespace Taurus.Synchronizers.AuthenticableUsers
{
    /// <summary>
    /// An abstract class that describes an authenticable user
    /// </summary>
    /// <typeparam name="TAuthenticableUser">Authenticable user type</typeparam>
    /// <typeparam name="TAuthenticableUsersSynchronizer">Authenticable users synchronizer type</typeparam>
    public abstract class AAuthenticableUser<TAuthenticableUser, TAuthenticableUsersSynchronizer> :
        AUser<TAuthenticableUser, TAuthenticableUsersSynchronizer>,
        IAuthenticableUser<TAuthenticableUser, TAuthenticableUsersSynchronizer>
        where TAuthenticableUser : AAuthenticableUser<TAuthenticableUser, TAuthenticableUsersSynchronizer>
        where TAuthenticableUsersSynchronizer : IAuthenticableUsersSynchronizer<TAuthenticableUsersSynchronizer, TAuthenticableUser>
    {
        /// <summary>
        /// Is user authenticated
        /// </summary>
        public bool IsAuthenticated { get; private set; }

        /// <summary>
        /// Constructs a new authenticable user
        /// </summary>
        /// <param name="userGUID">User GUID</param>
        /// <param name="peer">Peer</param>
        /// <param name="authenticableUsersSynchronizer">Authenticable users synchronizer</param>
        protected AAuthenticableUser(UserGUID userGUID, IPeer peer, TAuthenticableUsersSynchronizer authenticableUsersSynchronizer) :
            base(userGUID, peer, authenticableUsersSynchronizer)
        {
            // ...
        }

        /// <summary>
        /// Flags this user as authenticated
        /// </summary>
        void IAuthenticableUser<TAuthenticableUser, TAuthenticableUsersSynchronizer>.FlagAsAuthenticated() => IsAuthenticated = true;
    }
}
