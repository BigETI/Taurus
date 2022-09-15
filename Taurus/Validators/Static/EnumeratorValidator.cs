using System;
using System.Collections.Generic;

/// <summary>
/// Taurus validators namespace
/// </summary>
namespace Taurus.Validators
{
    /// <summary>
    /// A class that contains functionalities to validate enumerators
    /// </summary>
    public static class EnumeratorValidator
    {
        /// <summary>
        /// Is the specified enumerator valid
        /// </summary>
        /// <typeparam name="TEnumerator">Enumerator type</typeparam>
        /// <param name="enumerator">Enumerator</param>
        /// <param name="invalidEnumerator">Invalid enumerator</param>
        /// <returns>"true" if enumerator is valid, otherwise "false"</returns>
        public static bool IsEnumeratorValid<TEnumerator>(TEnumerator enumerator, TEnumerator invalidEnumerator) where TEnumerator : Enum =>
            !enumerator.Equals(invalidEnumerator);

        /// <summary>
        /// Are the specified enumerators valid
        /// </summary>
        /// <typeparam name="TEnumerator">Enumerator type</typeparam>
        /// <param name="enumerators">Enumerators</param>
        /// <param name="invalidEnumerator">Invalid enumerator</param>
        /// <returns>"true" if the specified enumerators are valid, otherwise "false"</returns>
        public static bool AreEnumeratorsValid<TEnumerator>(IEnumerable<TEnumerator>? enumerators, TEnumerator invalidEnumerator) where TEnumerator : Enum =>
            Validator.IsCollectionValid(enumerators, (enumerator) => IsEnumeratorValid(enumerator, invalidEnumerator));

        /// <summary>
        /// Validates the specified enumerator
        /// </summary>
        /// <typeparam name="TEnumerator">ENumerator type</typeparam>
        /// <param name="enumerator">Enumerator</param>
        /// <param name="invalidEnumerator">Invalid enumerator</param>
        /// <param name="parameterName">Parameter name</param>
        /// <exception cref="ValidationException{TEnumerator}">When the specified enumerator is not valid</exception>
        public static void ValidateEnumerator<TEnumerator>(TEnumerator enumerator, TEnumerator invalidEnumerator, string parameterName)
            where TEnumerator : Enum =>
            Validator.Validate(enumerator, parameterName, (input) => IsEnumeratorValid(input, invalidEnumerator));

        /// <summary>
        /// Validates the specified enumerators 
        /// </summary>
        /// <typeparam name="TEnumerator">Enumerator type</typeparam>
        /// <param name="enumerators">Enumerators</param>
        /// <param name="invalidEnumerator">Invalid enumerator</param>
        /// <param name="parameterName">Parameter name</param>
        /// <exception cref="ValidationException{IEnumerable{TEnumerator}?}">When the specified enumerator is not valid</exception>
        public static void ValidateEnumerators<TEnumerator>(IEnumerable<TEnumerator>? enumerators, TEnumerator invalidEnumerator, string parameterName)
            where TEnumerator : Enum =>
            Validator.ValidateCollection(enumerators, parameterName, (enumerator) => IsEnumeratorValid(enumerator, invalidEnumerator));
    }
}
