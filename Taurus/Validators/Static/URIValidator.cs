using System;
using System.Collections.Generic;

/// <summary>
/// Taurus validators namespace
/// </summary>
namespace Taurus.Validators
{
    /// <summary>
    /// A class that contains functionalities to validate URIs
    /// </summary>
    public static class URIValidator
    {
        /// <summary>
        /// Is the specified URI valid
        /// </summary>
        /// <param name="uri">URI</param>
        /// <returns>"true" if the specified URI is valid, otherwise "false"</returns>
        public static bool IsURIValid(Uri? uri) => uri != null;

        /// <summary>
        /// Is the specified URI valid
        /// </summary>
        /// <param name="uri">URI</param>
        /// <returns>"true" if the specified URI is valid, otherwise "false"</returns>
        public static bool IsURIValid(string? uri) => (uri != null) && Uri.TryCreate(uri, UriKind.RelativeOrAbsolute, out _);

        /// <summary>
        /// Are the specified URIs valid
        /// </summary>
        /// <param name="uris">URIs</param>
        /// <returns>"true" if the specified URIs are valid, otherwise "false"</returns>
        public static bool AreURIsValid(IEnumerable<Uri?>? uris)
        {
            bool ret = false;
            if (uris != null)
            {
                ret = true;
                foreach (Uri? uri in uris)
                {
                    if (!IsURIValid(uri))
                    {
                        ret = false;
                        break;
                    }
                }
            }
            return ret;
        }

        /// <summary>
        /// Are the specified URIs valid
        /// </summary>
        /// <param name="uris">URIs</param>
        /// <returns>"true" if the specified URIs are valid, otherwise "false"</returns>
        public static bool AreURIsValid(IEnumerable<string?>? uris)
        {
            bool ret = false;
            if (uris != null)
            {
                ret = true;
                foreach (string? uri in uris)
                {
                    if (!IsURIValid(uri))
                    {
                        ret = false;
                        break;
                    }
                }
            }
            return ret;
        }

        /// <summary>
        /// Validates the specified URI
        /// </summary>
        /// <param name="uri">URI</param>
        /// <param name="parameterName">Parameter name</param>
        public static void ValidateURI(Uri? uri, string parameterName) => Validator.Validate(uri, parameterName, IsURIValid);

        /// <summary>
        /// Validates the specified URI
        /// </summary>
        /// <param name="uri">URI</param>
        /// <param name="parameterName">Parameter name</param>
        public static void ValidateURI(string? uri, string parameterName) => Validator.Validate(uri, parameterName, IsURIValid);

        /// <summary>
        /// Validates the specified URIs
        /// </summary>
        /// <param name="uris">URIs</param>
        /// <param name="parameterName">Parameter name</param>
        public static void ValidateURIs(IEnumerable<Uri?>? uris, string parameterName) => Validator.Validate(uris, parameterName, AreURIsValid);

        /// <summary>
        /// Validates the specified URIs
        /// </summary>
        /// <param name="uris">URIs</param>
        /// <param name="parameterName">Parameter name</param>
        public static void ValidateURIs(IEnumerable<string?>? uris, string parameterName) => Validator.Validate(uris, parameterName, AreURIsValid);
    }
}
