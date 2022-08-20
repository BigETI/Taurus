/// <summary>
/// Taurus synchronizers namespace
/// </summary>
namespace Taurus.Synchronizers
{
    /// <summary>
    /// An interface that represents a peer message parser
    /// </summary>
    /// <typeparam name="TMessageData">Message data type</typeparam>
    public interface IPeerMessageParser<TMessageData> : IBasePeerMessageParser where TMessageData : IBaseMessageData
    {
        /// <summary>
        /// Gets invoked when a peer message has been parsed
        /// </summary>
        event PeerMessageParsedDelegate<TMessageData> OnPeerMessageParsed;

        /// <summary>
        /// Gets invoked when validating a peer message has failed
        /// </summary>
        event PeerMessageValidationFailedDelegate<TMessageData> OnPeerMessageValidationFailed;

        /// <summary>
        /// Gets invoked when parsing a peer message has failed
        /// </summary>
        event PeerMessageParseFailedDelegate OnPeerMessageParseFailed;
    }
}
