using System;
using System.Threading.Tasks;
using Taurus.Connectors;
using Taurus.Validators;

/// <summary>
/// Taurus synchronizers authenticated users namespace
/// </summary>
namespace Taurus.Synchronizers.AuthenticatedUsers
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
        /// Synchronizer
        /// </summary>
        public ISynchronizer Synchronizer { get; }

        /// <summary>
        /// Constructs a new user
        /// </summary>
        /// <param name="userGUID">User GUID</param>
        /// <param name="peer">Peer</param>
        /// <param name="synchronizer">Synchronizer</param>
        /// <exception cref="ArgumentException"></exception>
        public AUser(UserGUID userGUID, IPeer peer, ISynchronizer synchronizer)
        {
            GUIDValidator.ValidateGUID(userGUID, nameof(userGUID));
            UserGUID = userGUID;
            Peer = peer;
            Synchronizer = synchronizer;
        }

        /// <summary>
        /// Sends a message to this user asynchronously
        /// </summary>
        /// <typeparam name="TMessageData">Message data type</typeparam>
        /// <param name="message">Message</param>
        /// <returns>Task</returns>
        public Task SendMessageAsync<TMessageData>(TMessageData message) where TMessageData : IBaseMessageData =>
            Synchronizer.SendMessageToPeerAsync(Peer, message);

        /// <summary>
        /// Sends a ping message to this user asynchronously
        /// </summary>
        /// <returns>Task</returns>
        public Task SendPingMessageAsync() => Synchronizer.SendPeerPingMessageAsync(Peer);
    }
}
