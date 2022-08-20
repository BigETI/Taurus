using System;
using Taurus.Connectors;
using Taurus.Validators;

/// <summary>
/// Taurus synchronizers namespace
/// </summary>
namespace Taurus.Synchronizers
{
    /// <summary>
    /// An abstract class that describes an user
    /// </summary>
    public abstract class AUser : IUser
    {
        /// <summary>
        /// User GUID
        /// </summary>
        public UserGUID UserGUID { get; }

        /// <summary>
        /// Peer
        /// </summary>
        public IPeer Peer { get; }

        /// <summary>
        /// Constructs a new user
        /// </summary>
        /// <param name="userGUID">User GUID</param>
        /// <param name="peer">Peer</param>
        /// <exception cref="ArgumentException"></exception>
        public AUser(UserGUID userGUID, IPeer peer)
        {
            GUIDValidator.ValidateGUID(userGUID, nameof(userGUID));
            UserGUID = userGUID;
            Peer = peer;
        }
    }
}
