using Newtonsoft.Json;
using Taurus.JSONConverters;

/// <summary>
/// Taurus connectors namespace
/// </summary>
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
        /// Error
        /// </summary>
        Error,

        /// <summary>
        /// Object has been disposed
        /// </summary>
        Disposed,

        /// <summary>
        /// User has been disconnected
        /// </summary>
        Disconnected,

        /// <summary>
        /// User has timed out
        /// </summary>
        TimedOut,

        /// <summary>
        /// User has quit the lobby
        /// </summary>
        Quit,

        /// <summary>
        /// Kicked
        /// </summary>
        Kicked,

        /// <summary>
        /// Denied
        /// </summary>
        Denied,

        /// <summary>
        /// Lobby has been closed
        /// </summary>
        LobbyClosed,

        /// <summary>
        /// User has been deleted
        /// </summary>
        Deleted
    }
}
