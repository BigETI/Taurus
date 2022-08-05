using Taurus.Connectors;

namespace Taurus.JSONConverters
{
    /// <summary>
    /// A class used for converting disconnection reason to JSON and vice versa
    /// </summary>
    internal class DisconnectionReasonJSONConverter : EnumeratorValueJSONConverter<EDisconnectionReason>
    {
        /// <summary>
        /// Constructs an disconection reason JSON converter
        /// </summary>
        public DisconnectionReasonJSONConverter() : base(EDisconnectionReason.Invalid)
        {
            // ...
        }
    }
}
