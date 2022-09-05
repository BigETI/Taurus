using Taurus.Compressors;
using Taurus.Connectors.Local;
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
        /// Creates a new local connector
        /// </summary>
        /// <param name="onHandlePeerConnectionAttempt">Gets invoked when a peer conection attempt needs to be handled</param>
        /// <returns>New local connector</returns>
        public static ILocalConnector CreateNewLocalConnector(HandlePeerConnectionAttemptDelegate onHandlePeerConnectionAttempt) =>
            CreateNewLocalConnector(onHandlePeerConnectionAttempt, null, null);

        /// <summary>
        /// Creates a new local connector
        /// </summary>
        /// <param name="onHandlePeerConnectionAttempt">Gets invoked when a peer conection attempt needs to be handled</param>
        /// <param name="fragmenter">Fragmenter</param>
        /// <returns>New local connector</returns>
        public static ILocalConnector CreateNewLocalConnector(HandlePeerConnectionAttemptDelegate onHandlePeerConnectionAttempt, IFragmenter fragmenter) =>
            CreateNewLocalConnector(onHandlePeerConnectionAttempt, fragmenter, null);

        /// <summary>
        /// Creates a new local connector
        /// </summary>
        /// <param name="onHandlePeerConnectionAttempt">Gets invoked when a peer conection attempt needs to be handled</param>
        /// <param name="compressor">Compressor</param>
        /// <returns>New local connector</returns>
        public static ILocalConnector CreateNewLocalConnector(HandlePeerConnectionAttemptDelegate onHandlePeerConnectionAttempt, ICompressor? compressor) =>
            CreateNewLocalConnector(onHandlePeerConnectionAttempt, null, compressor);

        /// <summary>
        /// Creates a new local connector
        /// </summary>
        /// <param name="onHandlePeerConnectionAttempt">Gets invoked when a peer conection attempt needs to be handled</param>
        /// <param name="fragmenter">Fragmenter</param>
        /// <param name="compressor">Compressor</param>
        /// <returns>New local connector</returns>
        public static ILocalConnector CreateNewLocalConnector
        (
            HandlePeerConnectionAttemptDelegate onHandlePeerConnectionAttempt,
            IFragmenter? fragmenter,
            ICompressor? compressor
        ) => new LocalConnector(onHandlePeerConnectionAttempt, fragmenter, compressor);
    }
}
