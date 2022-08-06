using System;
using Taurus.Connectors;

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
        public Guid GUID { get; }

        /// <summary>
        /// Peer
        /// </summary>
        public IPeer Peer { get; }

        /// <summary>
        /// COnstructs a new user
        /// </summary>
        /// <param name="guid">User GUID</param>
        /// <param name="peer">Peer</param>
        /// <exception cref="ArgumentException"></exception>
        public AUser(Guid guid, IPeer peer)
        {
            if (guid == Guid.Empty)
            {
                throw new ArgumentException("User GUID can not be empty.", nameof(guid));
            }
            GUID = guid;
            Peer = peer;
        }
    }
}
