using System;
using Taurus.Validators;

/// <summary>
/// Taurus GUIDs namespace
/// </summary>
namespace Taurus.GUIDs
{
    /// <summary>
    /// An abstract class that describes a GUID
    /// </summary>
    /// <typeparam name="TGUID">GUID type</typeparam>
    public abstract class AGUID<TGUID> : IGUID<TGUID> where TGUID : IGUID<TGUID>
    {
        /// <summary>
        /// GUID
        /// </summary>
        public Guid GUID { get; }

        /// <summary>
        /// Is GUID empty
        /// </summary>
        public bool IsEmpty => GUID == Guid.Empty;

        /// <summary>
        /// Is this object valid
        /// </summary>
        public virtual bool IsValid => GUIDValidator.IsGUIDValid(GUID);

        /// <summary>
        /// Constructs a new GUID
        /// </summary>
        /// <param name="guid">GUID</param>
        protected AGUID(Guid guid)
        {
            GUIDValidator.ValidateGUID(guid, nameof(guid));
            GUID = guid;
        }

        /// <summary>
        /// Constructs a new GUID
        /// </summary>
        /// <param name="guid">GUID</param>
        /// <param name="isValidatingGUID">Is validating GUID</param>
        protected AGUID(Guid guid, bool isValidatingGUID)
        {
            if (isValidatingGUID)
            {
                GUIDValidator.ValidateGUID(guid, nameof(guid));
            }
            GUID = guid;
        }

        /// <summary>
        /// Compares this GUID to another GUID
        /// </summary>
        /// <param name="other">Other GUID</param>
        /// <returns>Comparison result</returns>
        public virtual int CompareTo(TGUID other) => GUID.CompareTo(other.GUID);

        /// <summary>
        /// Does this GUID equal to the specified GUID
        /// </summary>
        /// <param name="obj">Object</param>
        /// <returns>"true" if both GUIDs are equivalent, otherwise "false"</returns>
        public virtual bool Equals(TGUID other) => GUID == other?.GUID;

        /// <summary>
        /// Does this peer GUID equal to the specified object
        /// </summary>
        /// <param name="obj">Object</param>
        /// <returns>"true" if both objects are equivalent, otherwise "false"</returns>
        public override bool Equals(object? obj) => (obj is TGUID guid) && (GUID == guid.GUID);

        /// <summary>
        /// Gets the has code of this object
        /// </summary>
        /// <returns>Hash code of this object</returns>
        public override int GetHashCode() => GUID.GetHashCode();

        /// <summary>
        /// Gets the string representation of this object
        /// </summary>
        /// <returns>String representation</returns>
        public override string ToString() => GUID.ToString();
    }
}
