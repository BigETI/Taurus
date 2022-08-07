using Taurus.Synchronizers;

/// <summary>
/// Taurus JSON converters
/// </summary>
namespace Taurus.JSONConverters
{
    /// <summary>
    /// A class used for converting error types to JSON and vice versa
    /// </summary>
    internal class ErrorTypeJSONConverter : EnumeratorValueJSONConverter<EErrorType>
    {
        /// <summary>
        /// Constructs a new error type JSON converter
        /// </summary>
        public ErrorTypeJSONConverter() : base(EErrorType.Invalid)
        {
            // ...
        }
    }
}
