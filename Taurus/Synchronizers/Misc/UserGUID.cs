using System;
using Taurus.GUIDs;

namespace Taurus.Synchronizers
{
    /// <summary>
    /// A class that describes an user GUID
    /// </summary>
    public sealed class UserGUID : AGUID<UserGUID>
    {
        /// <summary>
        /// Constructs a new user GUID
        /// </summary>
        /// <param name="guid">GUID</param>
        public UserGUID(Guid guid) : base(guid)
        {
            // ...
        }

        /// <summary>
        /// Does this user GUID equal to the specified object
        /// </summary>
        /// <param name="obj">Object</param>
        /// <returns>"true" if both objects are equivalent, otherwise "false"</returns>
        public override bool Equals(object? obj) => base.Equals(obj);

        /// <summary>
        /// Gets the hash code of this object
        /// </summary>
        /// <returns>Hash code of this object</returns>
        public override int GetHashCode() => base.GetHashCode();

        /// <summary>
        /// Are the specified user GUIDs equivalent
        /// </summary>
        /// <param name="left">Left user GUID</param>
        /// <param name="right">Right user GUID</param>
        /// <returns>"true" if the specified user GUIDs are equivalent, otherwise "false"</returns>
        public static bool operator ==(UserGUID? left, UserGUID? right) => left?.GUID == right?.GUID;

        /// <summary>
        /// Are the specified user GUIDs not equivalent
        /// </summary>
        /// <param name="left">Left user GUID</param>
        /// <param name="right">Right user GUID</param>
        /// <returns>"true" if the specified user GUIDs are not equivalent, otherwise "false"</returns>
        public static bool operator !=(UserGUID? left, UserGUID? right) => left?.GUID != right?.GUID;
    }
}
