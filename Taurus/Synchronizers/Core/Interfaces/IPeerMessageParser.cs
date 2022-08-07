/// <summary>
/// Taurus synchronizers namespace
/// </summary>
namespace Taurus.Synchronizers
{
    /// <summary>
    /// An interface that represents a peer message parser
    /// </summary>
    /// <typeparam name="T">Message type</typeparam>
    public interface IPeerMessageParser<T> : IBaseMessageParser where T : IBaseMessageData
    {
        /// <summary>
        /// Gets invoked when a peer message has been parsed
        /// </summary>
        event PeerMessageParsedDelegate<T> OnPeerMessageParsed;

        /// <summary>
        /// Gets invoked when validating a peer message has failed
        /// </summary>
        event PeerMessageValidationFailedDelegate<T>? OnPeerMessageValidationFailed;

        /// <summary>
        /// Gets invoked when parsing a peer message has failed
        /// </summary>
        event PeerMessageParseFailedDelegate? OnPeerMessageParseFailed;
    }
}
