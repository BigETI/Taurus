/// <summary>
/// Taurus synchronizers namespace
/// </summary>
namespace Taurus.Synchronizers
{
    /// <summary>
    /// An interface that represents a peer message parser
    /// </summary>
    /// <typeparam name="TUser">User type</typeparam>
    /// <typeparam name="TSynchronizer">Synchronizer type</typeparam>
    /// <typeparam name="TMessageData">Message data type</typeparam>
    public interface IUserMessageParser<TUser, TSynchronizer, TMessageData> : IBaseUserMessageParser<TUser, TSynchronizer>
        where TUser : IUser<TUser, TSynchronizer>
        where TSynchronizer : ISynchronizer<TSynchronizer, TUser>
        where TMessageData : IBaseMessageData
    {
        /// <summary>
        /// Gets invoked when a peer message has been parsed
        /// </summary>
        event UserMessageParsedDelegate<TUser, TSynchronizer, TMessageData> OnUserMessageParsed;

        /// <summary>
        /// Gets invoked when validating a peer message has failed
        /// </summary>
        event UserMessageValidationFailedDelegate<TUser, TSynchronizer, TMessageData> OnUserMessageValidationFailed;

        /// <summary>
        /// Gets invoked when parsing a peer message has failed
        /// </summary>
        event UserMessageParseFailedDelegate<TUser, TSynchronizer> OnUserMessageParseFailed;
    }
}
