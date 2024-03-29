﻿using System.Collections.Generic;

namespace Taurus.Validators
{
    /// <summary>
    /// A class that contains functionalities to validate strings
    /// </summary>
    public static class StringValidator
    {
        /// <summary>
        /// Is the specified string not empty or has no whitespaces
        /// </summary>
        /// <param name="stringValue">String value</param>
        /// <returns>"true" if the specified string is not empty or has no whitespaces, otherwise "false"</returns>
        public static bool IsStringNotEmptyOrHasNoWhitespaces(string? stringValue) => !string.IsNullOrWhiteSpace(stringValue);

        /// <summary>
        /// Are the specified strings not empty or have no whitespaces
        /// </summary>
        /// <param name="strings">Strings</param>
        /// <returns>"true" if the specified strings are not empty or have no whitespaces, otherwise "false"</returns>
        public static bool AreStringsNotEmptyOrHaveNoWhitespaces(IEnumerable<string?>? strings) =>
            Validator.IsCollectionValid(strings, IsStringNotEmptyOrHasNoWhitespaces);

        /// <summary>
        /// Validates that the specified string is not empty or has no whitespaces
        /// </summary>
        /// <param name="stringValue">String value</param>
        /// <param name="parameterName">Parameter name</param>
        /// <exception cref="ValidationException{TInput}">When the specified string is not empty or has no whitespaces</exception>
        public static void ValidateStringIsNotEmptyOrHasNoWhitespaces(string? stringValue, string parameterName) =>
            Validator.Validate(stringValue, parameterName, IsStringNotEmptyOrHasNoWhitespaces);

        /// <summary>
        /// Validates that the specified strings are not empty or have no whitespaces
        /// </summary>
        /// <param name="strings">Strings</param>
        /// <param name="parameterName">Parameter name</param>
        /// <exception cref="ValidationException{TInput}">When the specified strings are not empty or have no whitespaces</exception>
        public static void ValidateStringsAreNotEmptyOrHaveNoWhitespaces(IEnumerable<string?>? strings, string parameterName) =>
            Validator.ValidateCollection(strings, parameterName, IsStringNotEmptyOrHasNoWhitespaces);
    }
}
