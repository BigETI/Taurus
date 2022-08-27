/// <summary>
/// Taurus synchronizers authenticable users namespace
/// </summary>
namespace Taurus.Synchronizers.AuthenticableUsers
{
    /// <summary>
    /// An interface that represents an authenticated users synchronizer
    /// </summary>
    /// <typeparam name="TAuthenticableUsersSynchronizer">Authenticable users synchronizer type</typeparam>
    /// <typeparam name="TAuthenticableUser">Authenticable user type</typeparam>
    public interface IAuthenticableUsersSynchronizer<TAuthenticableUsersSynchronizer, TAuthenticableUser> :
        ISynchronizer<TAuthenticableUsersSynchronizer, TAuthenticableUser>
        where TAuthenticableUsersSynchronizer : IAuthenticableUsersSynchronizer<TAuthenticableUsersSynchronizer, TAuthenticableUser>
        where TAuthenticableUser : IAuthenticableUser<TAuthenticableUser, TAuthenticableUsersSynchronizer>
    {
        /// <summary>
        /// Gets invoked when an user has been authenticated
        /// </summary>
        event UserAuthenticatedDelegate<TAuthenticableUser, TAuthenticableUsersSynchronizer>? OnUserAuthenticated;

        /// <summary>
        /// Gets invoked when a peer user has been authenticated
        /// </summary>
        event UserAuthenticationFailedDelegate<TAuthenticableUser, TAuthenticableUsersSynchronizer>? OnUserAuthenticationFailed;

        /// <summary>
        /// Adds an authenticated user message parser
        /// </summary>
        /// <typeparam name="TMessageData">Message data type</typeparam>
        /// <param name="onAuthenticatedUserMessageParsed">Gets invoked when an authenticated user message has been parsed</param>
        /// <param name="onPeerIsNotAuthenticated">Gets invoked when peer is not authenticated</param>
        /// <param name="onAuthenticatedUserMessageValidationFailed">Gets invoked when authenticated user message validation has failed</param>
        /// <param name="onAuthenticatedUserMessageParseFailed">Gets invoked when parsing an authenticated user message has failed</param>
        /// <returns>Peer message parser</returns>
        IUserMessageParser<TAuthenticableUser, TAuthenticableUsersSynchronizer, TMessageData> AddNewAuthenticatedUserMessageParser<TMessageData>
        (
            UserMessageParsedDelegate<TAuthenticableUser, TAuthenticableUsersSynchronizer, TMessageData> onAuthenticatedUserMessageParsed,
            UserMessageValidationFailedDelegate<TAuthenticableUser, TAuthenticableUsersSynchronizer, TMessageData> onAuthenticatedUserMessageValidationFailed,
            UserMessageParseFailedDelegate<TAuthenticableUser, TAuthenticableUsersSynchronizer> onAuthenticatedUserMessageParseFailed,
            UserIsNotAuthenticatedDelegate<TAuthenticableUser, TAuthenticableUsersSynchronizer> onPeerIsNotAuthenticated
        ) where TMessageData : IBaseMessageData;

        /// <summary>
        /// Adds a new automatic user message parser
        /// </summary>
        /// <typeparam name="TMessageData">Message data type</typeparam>
        /// <param name="onAuthenticatedUserMessageParsed">Gets invoked when an authenticated user message has been parsed</param>
        /// <param name="isFatal">Is validation fail or error fatal</param>
        /// <returns>Message parser</returns>
        IUserMessageParser<TAuthenticableUser, TAuthenticableUsersSynchronizer, TMessageData> AddNewAutomaticAuthenticatedUserMessageParser<TMessageData>
        (
            UserMessageParsedDelegate<TAuthenticableUser, TAuthenticableUsersSynchronizer, TMessageData> onAuthenticatedUserMessageParsed,
            bool isFatal
        ) where TMessageData : IBaseMessageData;

        /// <summary>
        /// Adds a new automatic user message parser
        /// </summary>
        /// <typeparam name="TMessageData">Message data type</typeparam>
        /// <param name="onAuthenticatedUserMessageParsed">Gets invoked when an authenticated user message has been parsed</param>
        /// <returns>Message parser</returns>
        IUserMessageParser<TAuthenticableUser, TAuthenticableUsersSynchronizer, TMessageData>
            AddNewAutomaticAuthenticatedUserMessageParser<TMessageData>
        (
            UserMessageParsedDelegate<TAuthenticableUser, TAuthenticableUsersSynchronizer, TMessageData> onAuthenticatedUserMessageParsed
        ) where TMessageData : IBaseMessageData;

        /// <summary>
        /// Adds a new automatic user message parser that is fatal on validation fail or error
        /// </summary>
        /// <typeparam name="TMessageData">Message data type</typeparam>
        /// <param name="onPeerMessageParsed">Gets invoked when a peer message has been parsed</param>
        /// <returns>Message parser</returns>
        IUserMessageParser<TAuthenticableUser, TAuthenticableUsersSynchronizer, TMessageData>
            AddNewAutomaticAuthenticatedUserMessageParserWithFatality<TMessageData>
        (
            UserMessageParsedDelegate<TAuthenticableUser, TAuthenticableUsersSynchronizer, TMessageData> onPeerMessageParsed
        ) where TMessageData : IBaseMessageData;
    }
}
