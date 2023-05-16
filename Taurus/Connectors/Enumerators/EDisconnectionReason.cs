using Newtonsoft.Json;
using Taurus.JSONConverters;

namespace Taurus.Connectors
{
    /// <summary>
    /// Disconnection reason enumerator
    /// </summary>
    [JsonConverter(typeof(DisconnectionReasonJSONConverter))]
    public enum EDisconnectionReason
    {
        /// <summary>
        /// Invalid disconnection reason
        /// </summary>
        Invalid,

        /// <summary>
        /// An error has occured
        /// </summary>
        Error,

        /// <summary>
        /// Peer conection has been denied
        /// </summary>
        Denied,

        /// <summary>
        /// Object has been disposed
        /// </summary>
        Disposed,

        /// <summary>
        /// Peer has timed out
        /// </summary>
        TimedOut,

        /// <summary>
        /// Peer has quit
        /// </summary>
        Quit,

        /// <summary>
        /// Peer has been kicked
        /// </summary>
        Kicked
    }
}
