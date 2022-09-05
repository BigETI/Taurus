using Taurus.Compressors;
using Taurus.Connectors.ENet;
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
        /// Creates a new ENet connector
        /// </summary>
        /// <param name="timeoutTime">Timeout time in milliseconds</param>
        /// <param name="onHandlePeerConnectionAttempt">Gets invoked when a peer conection attempt needs to be handled</param>
        /// <returns>New ENet connector</returns>
        public static IENetConnector CreateNewENetConnector
        (
            uint timeoutTime,
            HandlePeerConnectionAttemptDelegate onHandlePeerConnectionAttempt
        ) => CreateNewENetConnector(timeoutTime, onHandlePeerConnectionAttempt, null, null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeoutTime">Timeout time in milliseconds</param>
        /// <param name="onHandlePeerConnectionAttempt"></param>
        /// <param name="fragmenter">Fragmenter</param>
        /// <returns>New ENet connector</returns>
        public static IENetConnector CreateNewENetConnector
        (
            uint timeoutTime,
            HandlePeerConnectionAttemptDelegate onHandlePeerConnectionAttempt,
            IFragmenter fragmenter
        ) => CreateNewENetConnector(timeoutTime, onHandlePeerConnectionAttempt, fragmenter, null);

        /// <summary>
        /// Creates a new ENet connector
        /// </summary>
        /// <param name="timeoutTime">Timeout time in milliseconds</param>
        /// <param name="onHandlePeerConnectionAttempt">Gets invoked when a peer conection attempt needs to be handled</param>
        /// <param name="compressor">Compressor</param>
        /// <returns>New ENet connector</returns>
        public static IENetConnector CreateNewENetConnector
        (
            uint timeoutTime,
            HandlePeerConnectionAttemptDelegate onHandlePeerConnectionAttempt,
            ICompressor? compressor
        ) => CreateNewENetConnector(timeoutTime, onHandlePeerConnectionAttempt, null, compressor);

        /// <summary>
        /// Creates a new ENet connector
        /// </summary>
        /// <param name="timeoutTime">Timeout time in milliseconds</param>
        /// <param name="onHandlePeerConnectionAttempt">Gets invoked when a peer conection attempt needs to be handled</param>
        /// <param name="fragmenter">Fragmenter</param>
        /// <param name="compressor">Compressor</param>
        /// <returns>New ENet connector</returns>
        public static IENetConnector CreateNewENetConnector
        (
            uint timeoutTime,
            HandlePeerConnectionAttemptDelegate onHandlePeerConnectionAttempt,
            IFragmenter? fragmenter,
            ICompressor? compressor
        ) => new ENetConnector(timeoutTime, onHandlePeerConnectionAttempt, fragmenter, compressor);
    }
}
