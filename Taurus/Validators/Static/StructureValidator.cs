using System.Collections.Generic;

/// <summary>
/// Taurus validators namespace
/// </summary>
namespace Taurus.Validators
{
    /// <summary>
    /// A class that contains functionalities to validate structures
    /// </summary>
    public static class StructureValidator
    {
        /// <summary>
        /// Is the specified validable valid
        /// </summary>
        /// <typeparam name="TValidable">Validable type</typeparam>
        /// <param name="validable">Validable</param>
        /// <returns>"true" if the specified validable is valid, otherwise "false"</returns>
        public static bool IsValid<TValidable>(TValidable validable) where TValidable : struct, IValidable =>
            validable.IsValid;

        /// <summary>
        /// Is the specified validable valid
        /// </summary>
        /// <typeparam name="TValidable">Validable type</typeparam>
        /// <param name="validable">Validable</param>
        /// <returns>"true" if the specified validable is valid, otherwise "false"</returns>
        public static bool IsValid<TValidable>(TValidable? validable) where TValidable : struct, IValidable =>
            (validable != null) && validable.Value.IsValid;

        /// <summary>
        /// Is the specified collection valid
        /// </summary>
        /// <typeparam name="TValidable">Validable type</typeparam>
        /// <param name="collection">Collection</param>
        /// <returns>"true" if the specified collection is valid, otherwise "false"</returns>
        public static bool IsCollectionValid<TValidable>(IEnumerable<TValidable>? collection) where TValidable : struct, IValidable
        {
            bool ret = false;
            if (collection != null)
            {
                ret = true;
                foreach (TValidable validable in collection)
                {
                    if (!IsValid(validable))
                    {
                        ret = false;
                        break;
                    }
                }
            }
            return ret;
        }

        /// <summary>
        /// Is the specified collection valid
        /// </summary>
        /// <typeparam name="TValidable">Validable type</typeparam>
        /// <param name="collection">Collection</param>
        /// <returns>"true" if the specified collection is valid, otherwise "false"</returns>
        public static bool IsCollectionValid<TValidable>(IEnumerable<TValidable?>? collection) where TValidable : struct, IValidable
        {
            bool ret = false;
            if (collection != null)
            {
                ret = true;
                foreach (TValidable? validable in collection)
                {
                    if (!IsValid(validable))
                    {
                        ret = false;
                        break;
                    }
                }
            }
            return ret;
        }

        /// <summary>
        /// Validates the specified validable
        /// </summary>
        /// <typeparam name="TValidable">Validable type</typeparam>
        /// <param name="validable">Validable</param>
        /// <param name="parameterName">Parameter name</param>
        /// <exception cref="ValidationException{TValidable}">When the specified validable is not valid</exception>
        public static void Validate<TValidable>(TValidable validable, string parameterName) where TValidable : struct, IValidable =>
            Validator.Validate(validable, parameterName, IsValid);

        /// <summary>
        /// Validates the specified validable
        /// </summary>
        /// <typeparam name="TValidable">Validable type</typeparam>
        /// <param name="validable">Validable</param>
        /// <param name="parameterName">Parameter name</param>
        /// <exception cref="ValidationException{TValidable?}">When the specified validable is not valid</exception>
        public static void Validate<TValidable>(TValidable? validable, string parameterName) where TValidable : struct, IValidable =>
            Validator.Validate(validable, parameterName, IsValid);

        /// <summary>
        /// Validates the specified collection
        /// </summary>
        /// <typeparam name="TValidable">Validable type</typeparam>
        /// <param name="collection">Collection</param>
        /// <param name="parameterName">Parameter name</param>
        /// <exception cref="ValidationException{IEnumerable{TValidable}?}">When the specified collection is not valid</exception>
        public static void ValidateCollection<TValidable>(IEnumerable<TValidable>? collection, string parameterName)
            where TValidable : struct, IValidable =>
            Validator.Validate(collection, parameterName, IsCollectionValid);

        /// <summary>
        /// Validates the specified collection
        /// </summary>
        /// <typeparam name="TValidable">Validable type</typeparam>
        /// <param name="collection">Collection</param>
        /// <param name="parameterName">Parameter name</param>
        /// <exception cref="ValidationException{IEnumerable{TValidable?}?}">When the specified collection is not valid</exception>
        public static void ValidateCollection<TValidable>(IEnumerable<TValidable?>? collection, string parameterName)
            where TValidable : struct, IValidable =>
            Validator.Validate(collection, parameterName, IsCollectionValid);
    }
}
