using System;

/// <summary>
/// Taurus validators namespace
/// </summary>
namespace Taurus.Validators
{
    /// <summary>
    /// A class that describes a validation exception
    /// </summary>
    /// <typeparam name="TInput">Input type</typeparam>
    internal class ValidationException<TInput> : Exception
    {
        /// <summary>
        /// Input
        /// </summary>
        public TInput Input { get; }

        /// <summary>
        /// Symbol name
        /// </summary>
        public string SymbolName { get; }

        /// <summary>
        /// Constructs a new validation exception
        /// </summary>
        /// <param name="input">Input</param>
        /// <param name="symbolName">Symbol name</param>
        public ValidationException(TInput input, string symbolName) : base($"\"{ symbolName }\" is not valid.")
        {
            Input = input;
            SymbolName = symbolName;
        }
    }
}
