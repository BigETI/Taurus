using Taurus.Connectors;

/// <summary>
/// Taurus JSON converters
/// </summary>
namespace Taurus.JSONConverters
{
    /// <summary>
    /// A class used for converting disconnection reason to JSON or BSON and vice versa
    /// </summary>
    internal class DisconnectionReasonJSONConverter : EnumeratorValueJSONConverter<EDisconnectionReason>
    {
        /// <summary>
        /// Constructs a new disconnection reason JSON converter
        /// </summary>
        public DisconnectionReasonJSONConverter() : base(EDisconnectionReason.Invalid)
        {
            // ...
        }
    }
}
