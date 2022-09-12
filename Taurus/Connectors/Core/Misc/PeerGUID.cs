using System;
using Taurus.GUIDs;

/// <summary>
/// Taurus connectors namespace
/// </summary>
namespace Taurus.Connectors
{
    /// <summary>
    /// A class that describes a peer GUID
    /// </summary>
    public sealed class PeerGUID : AGUID<PeerGUID>
    {
        /// <summary>
        /// Constructs a new peer GUID
        /// </summary>
        /// <param name="guid">GUID</param>
        public PeerGUID(Guid guid) : base(guid)
        {
            // ...
        }

        /// <summary>
        /// Does this peer GUID equal to the specified object
        /// </summary>
        /// <param name="obj">Object</param>
        /// <returns>"true" if both objects are equivalent, otherwise "false"</returns>
        public override bool Equals(object obj) => base.Equals(obj);

        /// <summary>
        /// Gets the hash code of this object
        /// </summary>
        /// <returns>Hash code of this object</returns>
        public override int GetHashCode() => base.GetHashCode();

        /// <summary>
        /// Are the specified peer GUIDs equivalent
        /// </summary>
        /// <param name="left">Left peer GUID</param>
        /// <param name="right">Right peer GUID</param>
        /// <returns>"true" if the specified peer GUIDs are equivalent, otherwise "false"</returns>
        public static bool operator ==(PeerGUID left, PeerGUID right) => left.Equals(right);

        /// <summary>
        /// Are the specified peer GUIDs not equivalent
        /// </summary>
        /// <param name="left">Left peer GUID</param>
        /// <param name="right">Right peer GUID</param>
        /// <returns>"true" if the specified peer GUIDs are not equivalent, otherwise "false"</returns>
        public static bool operator !=(PeerGUID left, PeerGUID right) => !left.Equals(right);
    }
}
