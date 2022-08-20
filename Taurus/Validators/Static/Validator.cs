using System.Collections.Generic;

/// <summary>
/// Taurus validators namespace
/// </summary>
namespace Taurus.Validators
{
    /// <summary>
    /// A class that contains functionalities to validate objects
    /// </summary>
    public static class Validator
    {
        /// <summary>
        /// Is the specified input not null
        /// </summary>
        /// <typeparam name="TInput">Input type</typeparam>
        /// <param name="input">Input</param>
        /// <returns>"true" if the specified input is not null, otherwise "false"</returns>
        public static bool IsNotNull<TInput>(TInput input) => input != null;

        /// <summary>
        /// Is the specified input valid
        /// </summary>
        /// <typeparam name="TInput">Input type</typeparam>
        /// <param name="input">Input</param>
        /// <param name="onValidate">Gets invoked when input needs to be validated</param>
        /// <returns>"true" if the specified input is valid, otherwise "false"</returns>
        public static bool IsValid<TInput>(TInput input, ValidateDelegate<TInput> onValidate) => onValidate(input);

        /// <summary>
        /// Is the spcified validable valid
        /// </summary>
        /// <typeparam name="TValidable">Validable type</typeparam>
        /// <param name="validable">Validable</param>
        /// <returns>"true" if the specified validable is valid, otherwise "false"</returns>
        public static bool IsValid<TValidable>(TValidable? validable) where TValidable : class, IValidable =>
            (validable != null) && validable.IsValid;

        /// <summary>
        /// Is the spcified collection valid
        /// </summary>
        /// <typeparam name="TValidable">Validable type</typeparam>
        /// <param name="collection">Collection</param>
        /// <returns>"true" if the specified collection is valid, otherwise "false"</returns>
        public static bool IsCollectionValid<TValidable>(IEnumerable<TValidable?>? collection) where TValidable : class, IValidable
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
        /// Validates the specified input
        /// </summary>
        /// <typeparam name="TInput">Input type</typeparam>
        /// <param name="input">Input</param>
        /// <param name="parameterName">Parameter name</param>
        /// <param name="onValidate">Gets invoked when input needs to be validated</param>
        /// <exception cref="ValidationException{TInput}">When the specified input is not valid</exception>
        public static void Validate<TInput>(TInput input, string parameterName, ValidateDelegate<TInput> onValidate)
        {
            if (!onValidate(input))
            {
                throw new ValidationException<TInput>(input, parameterName);
            }
        }

        /// <summary>
        /// Validates the specified input
        /// </summary>
        /// <typeparam name="TInput">Input type</typeparam>
        /// <param name="input">Input</param>
        /// <param name="parameterName">Parameter name</param>
        /// <exception cref="ValidationException{TInput}">When the specified input is not valid</exception>
        public static void ValidateIsNotNull<TInput>(TInput input, string parameterName) => Validate(input, parameterName, IsNotNull);

        /// <summary>
        /// Validates the specified validable
        /// </summary>
        /// <typeparam name="TValidable">Validable type</typeparam>
        /// <param name="validable">Validable</param>
        /// <param name="parameterName">Parameter name</param>
        /// <exception cref="ValidationException{TValidable?}">When the specified validable is not valid</exception>
        public static void Validate<TValidable>(TValidable? validable, string parameterName) where TValidable : class, IValidable =>
            Validate(validable, parameterName, IsValid);

        /// <summary>
        /// Validates the specified collection
        /// </summary>
        /// <typeparam name="TValidable">Validable type</typeparam>
        /// <param name="collection">Collection</param>
        /// <param name="parameterName">Parameter name</param>
        /// <exception cref="ValidationException{IEnumerable{Validable?}?}">When the specified collection is not valid</exception>
        public static void ValidateCollection<TValidable>(IEnumerable<TValidable?>? collection, string parameterName)
            where TValidable : class, IValidable =>
            Validate(collection, parameterName, IsCollectionValid);
    }
}
