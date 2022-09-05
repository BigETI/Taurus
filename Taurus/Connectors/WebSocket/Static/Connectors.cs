using Taurus.Compressors;
using Taurus.Connectors.WebSocket;
using Taurus.Fragmenters;

/// <summary>
/// Taurus connectors namespace
/// </summary>
namespace Taurus.Connectors
{
    /// <summary>
    /// A class that provides functionalities to create connectors
    /// </summary>
    public static partial class Connectors
    {
        /// <summary>
        /// Creates a new WebSocket connector
        /// </summary>
        /// <param name="onHandlePeerConnectionAttempt">Gets invoked when a peer conection attempt needs to be handled</param>
        /// <returns>New WebSocket connector</returns>
        public static IWebSocketConnector CreateNewWebSocketConnector(HandlePeerConnectionAttemptDelegate onHandlePeerConnectionAttempt) =>
            CreateNewWebSocketConnector(onHandlePeerConnectionAttempt, null, null);

        /// <summary>
        /// Creates a new WebSocket connector
        /// </summary>
        /// <param name="onHandlePeerConnectionAttempt">Gets invoked when a peer conection attempt needs to be handled</param>
        /// <param name="fragmenter">Fragmenter</param>
        /// <returns>New WebSocket connector</returns>
        public static IWebSocketConnector CreateNewWebSocketConnector
        (
            HandlePeerConnectionAttemptDelegate onHandlePeerConnectionAttempt,
            IFragmenter fragmenter
        ) => CreateNewWebSocketConnector(onHandlePeerConnectionAttempt, fragmenter, null);

        /// <summary>
        /// Creates a new WebSocket connector
        /// </summary>
        /// <param name="onHandlePeerConnectionAttempt">Gets invoked when a peer conection attempt needs to be handled</param>
        /// <param name="compressor">Compressor</param>
        /// <returns>New WebSocket connector</returns>
        public static IWebSocketConnector CreateNewWebSocketConnector
        (
            HandlePeerConnectionAttemptDelegate onHandlePeerConnectionAttempt,
            ICompressor compressor
        ) => CreateNewWebSocketConnector(onHandlePeerConnectionAttempt, null, compressor);

        /// <summary>
        /// Creates a new WebSocket connector
        /// </summary>
        /// <param name="onHandlePeerConnectionAttempt">Gets invoked when a peer conection attempt needs to be handled</param>
        /// <param name="fragmenter">Fragmenter</param>
        /// <param name="compressor">Compressor</param>
        /// <returns>New WebSocket connector</returns>
        public static IWebSocketConnector CreateNewWebSocketConnector
        (
            HandlePeerConnectionAttemptDelegate onHandlePeerConnectionAttempt,
            IFragmenter? fragmenter,
            ICompressor? compressor
        ) => new WebSocketConnector(onHandlePeerConnectionAttempt, fragmenter, compressor);
    }
}
