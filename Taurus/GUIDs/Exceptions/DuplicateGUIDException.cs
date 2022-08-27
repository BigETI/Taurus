using System;
using System.Runtime.Serialization;

/// <summary>
/// Taurus GUIDs namespace
/// </summary>
namespace Taurus.GUIDs
{
    /// <summary>
    /// A class that describes a duplicate GUID exception
    /// </summary>
    public class DuplicateGUIDException : Exception
    {
        /// <summary>
        /// GUID
        /// </summary>
        public Guid GUID;

        /// <summary>
        /// Constructs a new duplicate GUID exception
        /// </summary>
        /// <param name="guid">GUID</param>
        public DuplicateGUIDException(Guid guid) => GUID = guid;

        /// <summary>
        /// Constructs a new duplicate GUID exception
        /// </summary>
        /// <param name="guid">GUID</param>
        /// <param name="message">Message</param>
        public DuplicateGUIDException(Guid guid, string message) : base(message) => GUID = guid;

        /// <summary>
        /// Constructs a new duplicate GUID exception
        /// </summary>
        /// <param name="guid">GUID</param>
        /// <param name="message">Message</param>
        /// <param name="innerException">Inner exception</param>
        public DuplicateGUIDException(Guid guid, string message, Exception innerException) : base(message, innerException) => GUID = guid;

        /// <summary>
        /// Constructs a new duplicate GUID exception
        /// </summary>
        /// <param name="info">Serialization information</param>
        /// <param name="context">Streaming context</param>
        protected DuplicateGUIDException(SerializationInfo info, StreamingContext context) : base(info, context) =>
                GUID = Guid.TryParse(info.GetString(nameof(GUID)), out Guid guid) ? guid : Guid.Empty;
    }

    /// <summary>
    /// A class that describes a duplicate GUID exception
    /// </summary>
    public class DuplicateGUIDException<TGUID> : Exception where TGUID : IGUID<TGUID>
    {
        /// <summary>
        /// GUID
        /// </summary>
        public TGUID GUID;

        /// <summary>
        /// Constructs a new duplicate GUID exception
        /// </summary>
        /// <param name="guid">GUID</param>
        public DuplicateGUIDException(TGUID guid) => GUID = guid;

        /// <summary>
        /// Constructs a new duplicate GUID exception
        /// </summary>
        /// <param name="guid">GUID</param>
        /// <param name="message">Message</param>
        public DuplicateGUIDException(TGUID guid, string message) : base(message) => GUID = guid;

        /// <summary>
        /// Constructs a new duplicate GUID exception
        /// </summary>
        /// <param name="guid">GUID</param>
        /// <param name="message">Message</param>
        /// <param name="innerException">Inner exception</param>
        public DuplicateGUIDException(TGUID guid, string message, Exception innerException) : base(message, innerException) => GUID = guid;

        /// <summary>
        /// Constructs a new duplicate GUID exception
        /// </summary>
        /// <param name="info">Serialization information</param>
        /// <param name="context">Streaming context</param>
        protected DuplicateGUIDException(SerializationInfo info, StreamingContext context) : base(info, context) =>
                GUID = (TGUID)Activator.CreateInstance(typeof(TGUID), new object[] { Guid.TryParse(info.GetString(nameof(GUID)), out Guid guid) ? guid : Guid.Empty });
    }
}
