using System.Collections.Generic;

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
        /// Is the specified validable valid
        /// </summary>
        /// <typeparam name="TValidable">Validable type</typeparam>
        /// <param name="validable">Validable</param>
        /// <returns>"true" if the specified validable is valid, otherwise "false"</returns>
        public static bool IsValid<TValidable>(TValidable? validable) where TValidable : class, IValidable =>
            (validable != null) && validable.IsValid;

        /// <summary>
        /// Is the specified collection valid
        /// </summary>
        /// <typeparam name="TCollectionElement">Collection element type</typeparam>
        /// <param name="collection">Collection</param>
        /// <param name="onValidateCollectionElement">Gets invoked when a collection element needs to be validated</param>
        /// <returns>"true" if the specified collection is valid, otherwise "false"</returns>
        public static bool IsCollectionValid<TCollectionElement>
        (
            IEnumerable<TCollectionElement>? collection,
            ValidateDelegate<TCollectionElement> onValidateCollectionElement
        )
        {
            bool ret = false;
            if (collection != null)
            {
                ret = true;
                foreach (TCollectionElement collection_element in collection)
                {
                    if (!onValidateCollectionElement(collection_element))
                    {
                        ret = false;
                        break;
                    }
                }
            }
            return ret;
        }

        /// <summary>
        /// Is the specified collection not null
        /// </summary>
        /// <typeparam name="TCollectionElement">Validable type</typeparam>
        /// <param name="collection">Collection</param>
        /// <returns>"true" if the specified collection is not null, otherwise "false"</returns>
        public static bool IsCollectionNotNull<TCollectionElement>(IEnumerable<TCollectionElement>? collection) =>
            IsCollectionValid(collection, (collectionElement) => collectionElement != null);

        /// <summary>
        /// Is the specified collection valid
        /// </summary>
        /// <typeparam name="TValidable">Validable type</typeparam>
        /// <param name="collection">Collection</param>
        /// <returns>"true" if the specified collection is valid, otherwise "false"</returns>
        public static bool IsCollectionValid<TValidable>(IEnumerable<TValidable?>? collection) where TValidable : class, IValidable =>
            IsCollectionValid(collection, (collectionElement) => IsValid(collectionElement));

        /// <summary>
        /// Is the specified element contained inside the specified collection
        /// </summary>
        /// <typeparam name="TCollectionElement">Collection element type</typeparam>
        /// <param name="collection">Collection</param>
        /// <param name="collectionElement">Collection element</param>
        /// <returns>"true" if the specified collection element is contained inside the specified collection, otherwise "false"</returns>
        public static bool IsElementContainedInCollection<TCollectionElement>(IEnumerable<TCollectionElement>? collection, TCollectionElement collectionElement)
        {
            bool ret = false;
            if (collection != null)
            {
                foreach (TCollectionElement collection_element in collection)
                {
                    if (Equals(collectionElement, collection_element))
                    {
                        ret = true;
                        break;
                    }
                }
            }
            return ret;
        }

        /// <summary>
        /// Is the specified collection valid and all of its elements unique
        /// </summary>
        /// <typeparam name="TCollectionElement">Collection element type</typeparam>
        /// <param name="collection">Collection</param>
        /// <returns>"true" if the specified collection is valid, otherwise "false"</returns>
        public static bool AreAllCollectionElementsUnique<TCollectionElement>(IEnumerable<TCollectionElement>? collection) =>
            IsCollectionValid
            (
                collection,
                (collectionElement) =>
                {
                    bool ret = true;
                    bool is_no_first_entry_found = true;
                    foreach (TCollectionElement collection_element in collection!)
                    {
                        if (Equals(collectionElement, collection_element))
                        {
                            if (is_no_first_entry_found)
                            {
                                is_no_first_entry_found = false;
                            }
                            else
                            {
                                ret = false;
                                break;
                            }
                        }
                    }
                    return ret;
                }
            );

        /// <summary>
        /// Is the specified collection valid and all of its elements unique
        /// </summary>
        /// <typeparam name="TCollectionElement">Collection element type</typeparam>
        /// <param name="collection">Collection</param>
        /// <returns>"true" if the specified collection is valid, otherwise "false"</returns>
        public static bool IsCollectionValidAndAllElementsUnique<TCollectionElement>(IEnumerable<TCollectionElement?>? collection)
            where TCollectionElement : class, IValidable =>
            IsCollectionValid
            (
                collection,
                (collectionElement) =>
                {
                    bool ret = IsValid(collectionElement);
                    if (ret)
                    {
                        bool is_no_first_entry_found = true;
                        foreach (TCollectionElement? collection_element in collection!)
                        {
                            if (Equals(collectionElement, collection_element))
                            {
                                if (is_no_first_entry_found)
                                {
                                    is_no_first_entry_found = false;
                                }
                                else
                                {
                                    ret = false;
                                    break;
                                }
                            }
                        }
                    }
                    return ret;
                }
            );

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
        /// <exception cref="ValidationException{TInput}">When the specified validable is not valid</exception>
        public static void Validate<TValidable>(TValidable? validable, string parameterName) where TValidable : class, IValidable =>
            Validate(validable, parameterName, IsValid);

        /// <summary>
        /// Validates the specified collection
        /// </summary>
        /// <typeparam name="TCollectionElement">Collection element type</typeparam>
        /// <param name="collection">Collection</param>
        /// <param name="parameterName">Parameter name</param>
        /// <param name="onValidateCollectionElement">GEts invoked when accollection element needs to be validated</param>
        /// <exception cref="ValidationException{TInput}">When the specified collection is not valid</exception>
        public static void ValidateCollection<TCollectionElement>
        (
            IEnumerable<TCollectionElement>? collection,
            string parameterName,
            ValidateDelegate<TCollectionElement> onValidateCollectionElement
        )
        {
            ValidateIsNotNull(collection, parameterName);
            foreach (TCollectionElement collection_element in collection!)
            {
                Validate(collection_element, parameterName, onValidateCollectionElement);
            }
        }

        /// <summary>
        /// Validates the specified collectionthat it is not null
        /// </summary>
        /// <typeparam name="TCollectionElement">Validable type</typeparam>
        /// <param name="collection">Collection</param>
        /// <param name="parameterName">Parameter name</param>
        /// <exception cref="ValidationException{TInput}">When the specified collection is not valid</exception>
        public static void ValidateCollectionIsNotNull<TCollectionElement>(IEnumerable<TCollectionElement>? collection, string parameterName) =>
            ValidateCollection(collection, parameterName, IsNotNull);

        /// <summary>
        /// Validates the specified collection
        /// </summary>
        /// <typeparam name="TValidable">Validable type</typeparam>
        /// <param name="collection">Collection</param>
        /// <param name="parameterName">Parameter name</param>
        /// <exception cref="ValidationException{TInput}">When the specified collection is not valid</exception>
        public static void ValidateCollection<TValidable>(IEnumerable<TValidable?>? collection, string parameterName)
            where TValidable : class, IValidable =>
            ValidateCollection(collection, parameterName, IsValid);

        /// <summary>
        /// Validates the specified collection that all of its elements are unique
        /// </summary>
        /// <typeparam name="TCollectionElement">Collection element type</typeparam>
        /// <param name="collection">Collection</param>
        /// <param name="collectionElement">Collection element</param>
        /// <param name="parameterName">Parameter name</param>
        /// <exception cref="ValidationException{TInput}">When the specified collection is not valid</exception>
        public static void ValidateCollectionElementIsContainedInsideCollection<TCollectionElement>
        (
            IEnumerable<TCollectionElement>? collection,
            TCollectionElement collectionElement,
            string parameterName
        ) => Validate(collection, parameterName, (input) => IsElementContainedInCollection(input, collectionElement));

        /// <summary>
        /// Validates the specified collection that all of its elements are unique
        /// </summary>
        /// <typeparam name="TCollectionElement">Collection element type</typeparam>
        /// <param name="collection">Collection</param>
        /// <param name="parameterName">Parameter name</param>
        /// <exception cref="ValidationException{TInput}">When the specified collection is not valid</exception>
        public static void ValidateAllCollectionElementsBeingUnique<TCollectionElement>(IEnumerable<TCollectionElement>? collection, string parameterName) =>
            Validate(collection, parameterName, AreAllCollectionElementsUnique);

        /// <summary>
        /// Validates the specified collection which all of its elements are also unique
        /// </summary>
        /// <typeparam name="TCollectionElement">Validable type</typeparam>
        /// <param name="collection">Collection</param>
        /// <param name="parameterName">Parameter name</param>
        /// <exception cref="ValidationException{TInput}">When the specified collection is not valid</exception>
        public static void ValidateCollectionWithOnlyUniqueElements<TCollectionElement>(IEnumerable<TCollectionElement?>? collection, string parameterName)
            where TCollectionElement : class, IValidable =>
            Validate(collection, parameterName, IsCollectionValidAndAllElementsUnique);
    }
}
