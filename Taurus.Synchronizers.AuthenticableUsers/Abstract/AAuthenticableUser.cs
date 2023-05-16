using Taurus.Connectors;
using Taurus.Serializers;

namespace Taurus.Synchronizers.AuthenticableUsers
{
    /// <summary>
    /// An abstract class that describes an authenticable user
    /// </summary>
    public abstract class AAuthenticableUser : AUser, IAuthenticableUser
    {
        /// <summary>
        /// Is user authenticated
        /// </summary>
        public virtual bool IsAuthenticated { get; private set; }

        /// <summary>
        /// Constructs a new authenticable user
        /// </summary>
        /// <param name="userGUID">User GUID</param>
        /// <param name="peer">Peer</param>
        /// <param name="serializer">Serializer</param>
        protected AAuthenticableUser(UserGUID userGUID, IPeer peer, ISerializer serializer) : base(userGUID, peer, serializer)
        {
            // ...
        }

        /// <summary>
        /// Flags this user as authenticated
        /// </summary>
        void IAuthenticableUser.FlagAsAuthenticated() => IsAuthenticated = true;
    }
}
