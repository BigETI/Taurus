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
        /// <param name="eNetHostServiceTimeoutTime">ENet host service timeout time in milliseconds</param>
        /// <param name="maximalPeerCount">Maximal peer count</param>
        /// <param name="onHandlePeerConnectionAttempt">Gets invoked when a peer conection attempt needs to be handled</param>
        /// <returns>New ENet connector</returns>
        public static IENetConnector CreateNewENetConnector
        (
            uint eNetHostServiceTimeoutTime,
            uint maximalPeerCount,
            HandlePeerConnectionAttemptDelegate onHandlePeerConnectionAttempt
        ) => CreateNewENetConnector(eNetHostServiceTimeoutTime, maximalPeerCount, onHandlePeerConnectionAttempt, null, null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eNetHostServiceTimeoutTime">ENet host service timeout time in milliseconds</param>
        /// <param name="maximalPeerCount">Maximal peer count</param>
        /// <param name="onHandlePeerConnectionAttempt"></param>
        /// <param name="fragmenter">Fragmenter</param>
        /// <returns>New ENet connector</returns>
        public static IENetConnector CreateNewENetConnector
        (
            uint eNetHostServiceTimeoutTime,
            uint maximalPeerCount,
            HandlePeerConnectionAttemptDelegate onHandlePeerConnectionAttempt,
            IFragmenter fragmenter
        ) => CreateNewENetConnector(eNetHostServiceTimeoutTime, maximalPeerCount, onHandlePeerConnectionAttempt, fragmenter, null);

        /// <summary>
        /// Creates a new ENet connector
        /// </summary>
        /// <param name="eNetHostServiceTimeoutTime">ENet host service timeout time in milliseconds</param>
        /// <param name="maximalPeerCount">Maximal peer count</param>
        /// <param name="onHandlePeerConnectionAttempt">Gets invoked when a peer conection attempt needs to be handled</param>
        /// <param name="compressor">Compressor</param>
        /// <returns>New ENet connector</returns>
        public static IENetConnector CreateNewENetConnector
        (
            uint eNetHostServiceTimeoutTime,
            uint maximalPeerCount,
            HandlePeerConnectionAttemptDelegate onHandlePeerConnectionAttempt,
            ICompressor compressor
        ) => CreateNewENetConnector(eNetHostServiceTimeoutTime, maximalPeerCount, onHandlePeerConnectionAttempt, null, compressor);

        /// <summary>
        /// Creates a new ENet connector
        /// </summary>
        /// <param name="eNetHostServiceTimeoutTime">ENet host service timeout time in milliseconds</param>
        /// <param name="maximalPeerCount">Maximal peer count</param>
        /// <param name="onHandlePeerConnectionAttempt">Gets invoked when a peer conection attempt needs to be handled</param>
        /// <param name="fragmenter">Fragmenter</param>
        /// <param name="compressor">Compressor</param>
        /// <returns>New ENet connector</returns>
        public static IENetConnector CreateNewENetConnector
        (
            uint eNetHostServiceTimeoutTime,
            uint maximalPeerCount,
            HandlePeerConnectionAttemptDelegate onHandlePeerConnectionAttempt,
            IFragmenter? fragmenter,
            ICompressor? compressor
        ) => new ENetConnector(eNetHostServiceTimeoutTime, maximalPeerCount, onHandlePeerConnectionAttempt, fragmenter, compressor);
    }
}
