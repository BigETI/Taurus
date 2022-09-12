using System;
using System.Runtime.Serialization;

/// <summary>
/// Taurus promises namespace
/// </summary>
namespace Taurus.Promises
{
    /// <summary>
    /// A class that describes an early promised value access exception
    /// </summary>
    public sealed class EarlyPromisedValueAccessException : Exception
    {
        /// <summary>
        /// Constructs a new early promised value access exception
        /// </summary>
        public EarlyPromisedValueAccessException() : base("Tried to access a promised value too early.")
        {
            // ...
        }

        /// <summary>
        /// Constructs a new early promised value access exception
        /// </summary>
        /// <param name="message">Message</param>
        public EarlyPromisedValueAccessException(string message) : base(message)
        {
            // ...
        }

        /// <summary>
        /// Constructs a new early promised value access exception
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="innerException">Inner exception</param>
        public EarlyPromisedValueAccessException(string message, Exception innerException) : base(message, innerException)
        {
            // ...
        }
    }
}
