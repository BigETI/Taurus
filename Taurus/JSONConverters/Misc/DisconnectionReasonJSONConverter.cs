using Taurus.Connectors;

namespace Taurus.JSONConverters
{
    /// <summary>
    /// A class used for converting disconnection reason to JSON or BSON and vice versa
    /// </summary>
    internal sealed class DisconnectionReasonJSONConverter : AEnumeratorValueJSONConverter<EDisconnectionReason>
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
