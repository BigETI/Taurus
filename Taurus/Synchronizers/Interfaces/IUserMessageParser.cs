namespace Taurus.Synchronizers
{
    /// <summary>
    /// An interface that represents a peer message parser
    /// </summary>
    /// <typeparam name="TUser">User type</typeparam>
    /// <typeparam name="TMessageData">Message data type</typeparam>
    public interface IUserMessageParser<TUser, TMessageData> : IBaseUserMessageParser<TUser> where TUser : IUser where TMessageData : IBaseMessageData
    {
        /// <summary>
        /// Gets invoked when a peer message has been parsed
        /// </summary>
        event UserMessageParsedDelegate<TUser, TMessageData> OnUserMessageParsed;

        /// <summary>
        /// Gets invoked when validating a peer message has failed
        /// </summary>
        event UserMessageValidationFailedDelegate<TUser, TMessageData> OnUserMessageValidationFailed;

        /// <summary>
        /// Gets invoked when parsing a peer message has failed
        /// </summary>
        event UserMessageParseFailedDelegate<TUser> OnUserMessageParseFailed;
    }
}
