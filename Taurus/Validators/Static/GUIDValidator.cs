using System;
using System.Collections.Generic;
using Taurus.GUIDs;

/// <summary>
/// Taurus validators namespace
/// </summary>
namespace Taurus.Validators
{
    /// <summary>
    /// A class that contains functionalities to validate GUIDs
    /// </summary>
    public static class GUIDValidator
    {
        /// <summary>
        /// Is the specified GUID valid
        /// </summary>
        /// <typeparam name="TGUID">GUID type</typeparam>
        /// <param name="guid">GUID</param>
        /// <returns>"true" if the specified GUID is valid, otherwise "false"</returns>
        public static bool IsGUIDValid<TGUID>(TGUID guid) where TGUID : IGUID<TGUID> => guid.IsValid;

        /// <summary>
        /// Is the specified GUID valid
        /// </summary>
        /// <param name="guid">GUID</param>
        /// <returns>"true" if the specified GUID is valid, otherwise "false"</returns>
        public static bool IsGUIDValid(Guid guid) => guid != Guid.Empty;

        /// <summary>
        /// Are the specified GUIDs valid
        /// </summary>
        /// <typeparam name="TGUID">GUID type</typeparam>
        /// <param name="guids">GUIDs</param>
        /// <returns>"true" if the specified GUIDs are valid, otherwise "false"</returns>
        public static bool AreGUIDsValid<TGUID>(IEnumerable<TGUID>? guids) where TGUID : IGUID<TGUID> =>
            Validator.IsCollectionValid(guids, IsGUIDValid);

        /// <summary>
        /// Are the specified GUIDs valid
        /// </summary>
        /// <param name="guids">GUIDs</param>
        /// <returns>"true" if the specified GUIDs are valid, otherwise "false"</returns>
        public static bool AreGUIDsValid(IEnumerable<Guid>? guids) =>
            Validator.IsCollectionValid(guids, IsGUIDValid);

        /// <summary>
        /// Validates the specified GUID
        /// </summary>
        /// <typeparam name="TGUID">GUID type</typeparam>
        /// <param name="guid">GUID</param>
        /// <param name="parameterName">Parameter name</param>
        /// <exception cref="ValidationException{TGUID}">When the specified GUID is not valid</exception>
        public static void ValidateGUID<TGUID>(TGUID guid, string parameterName) where TGUID : IGUID<TGUID> =>
            Validator.Validate(guid, parameterName, IsGUIDValid);

        /// <summary>
        /// Validates the specified GUID
        /// </summary>
        /// <param name="guid">GUID</param>
        /// <param name="parameterName">Parameter name</param>
        /// <exception cref="ValidationException{Guid}">When the specified GUID is not valid</exception>
        public static void ValidateGUID(Guid guid, string parameterName) =>
            Validator.Validate(guid, parameterName, IsGUIDValid);

        /// <summary>
        /// Validates the specified GUIDs
        /// </summary>
        /// <typeparam name="TGUID">GUID type</typeparam>
        /// <param name="guid">GUID</param>
        /// <param name="parameterName">Parameter name</param>
        /// <exception cref="ValidationException{IEnumerable{TGUID}}">When the specified GUIDs are not valid</exception>
        public static void ValidateGUIDs<TGUID>(IEnumerable<TGUID>? guid, string parameterName) where TGUID : IGUID<TGUID> =>
            Validator.ValidateCollection(guid, parameterName, IsGUIDValid);

        /// <summary>
        /// Validates the specified GUIDs
        /// </summary>
        /// <param name="guid">GUID</param>
        /// <param name="parameterName">Parameter name</param>
        /// <exception cref="ValidationException{IEnumerable{Guid}?}">When the specified GUIDs are not valid</exception>
        public static void ValidateGUIDs(IEnumerable<Guid>? guid, string parameterName) =>
            Validator.ValidateCollection(guid, parameterName, IsGUIDValid);
    }
}
