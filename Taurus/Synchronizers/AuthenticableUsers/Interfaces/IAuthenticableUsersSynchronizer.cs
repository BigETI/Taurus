﻿/// <summary>
/// Taurus synchronizers authenticable users namespace
/// </summary>
namespace Taurus.Synchronizers.AuthenticableUsers
{
    /// <summary>
    /// An interface that represents an authenticated users synchronizer
    /// </summary>
    /// <typeparam name="TAuthenticableUser">Authenticable user type</typeparam>
    public interface IAuthenticableUsersSynchronizer<TAuthenticableUser> : ISynchronizer<TAuthenticableUser> where TAuthenticableUser : IAuthenticableUser
    {
        /// <summary>
        /// Gets invoked when an user has been authenticated
        /// </summary>
        event UserAuthenticatedDelegate<TAuthenticableUser>? OnUserAuthenticated;

        /// <summary>
        /// Gets invoked when a peer user has been authenticated
        /// </summary>
        event UserAuthenticationFailedDelegate<TAuthenticableUser>? OnUserAuthenticationFailed;

        /// <summary>
        /// Adds an authenticated user message parser
        /// </summary>
        /// <typeparam name="TMessageData">Message data type</typeparam>
        /// <param name="onAuthenticatedUserMessageParsed">Gets invoked when an authenticated user message has been parsed</param>
        /// <param name="onPeerIsNotAuthenticated">Gets invoked when peer is not authenticated</param>
        /// <param name="onAuthenticatedUserMessageValidationFailed">Gets invoked when authenticated user message validation has failed</param>
        /// <param name="onAuthenticatedUserMessageParseFailed">Gets invoked when parsing an authenticated user message has failed</param>
        /// <returns>Peer message parser</returns>
        IUserMessageParser<TAuthenticableUser, TMessageData> AddNewAuthenticatedUserMessageParser<TMessageData>
        (
            UserMessageParsedDelegate<TAuthenticableUser, TMessageData> onAuthenticatedUserMessageParsed,
            UserMessageValidationFailedDelegate<TAuthenticableUser, TMessageData> onAuthenticatedUserMessageValidationFailed,
            UserMessageParseFailedDelegate<TAuthenticableUser> onAuthenticatedUserMessageParseFailed,
            UserIsNotAuthenticatedDelegate<TAuthenticableUser> onPeerIsNotAuthenticated
        ) where TMessageData : IBaseMessageData;

        /// <summary>
        /// Adds a new automatic user message parser
        /// </summary>
        /// <typeparam name="TMessageData">Message data type</typeparam>
        /// <param name="onAuthenticatedUserMessageParsed">Gets invoked when an authenticated user message has been parsed</param>
        /// <param name="isFatal">Is validation fail or error fatal</param>
        /// <returns>Message parser</returns>
        IUserMessageParser<TAuthenticableUser, TMessageData> AddNewAutomaticAuthenticatedUserMessageParser<TMessageData>
        (
            UserMessageParsedDelegate<TAuthenticableUser, TMessageData> onAuthenticatedUserMessageParsed,
            bool isFatal
        ) where TMessageData : IBaseMessageData;

        /// <summary>
        /// Adds a new automatic user message parser
        /// </summary>
        /// <typeparam name="TMessageData">Message data type</typeparam>
        /// <param name="onAuthenticatedUserMessageParsed">Gets invoked when an authenticated user message has been parsed</param>
        /// <returns>Message parser</returns>
        IUserMessageParser<TAuthenticableUser, TMessageData>
            AddNewAutomaticAuthenticatedUserMessageParser<TMessageData>
        (
            UserMessageParsedDelegate<TAuthenticableUser, TMessageData> onAuthenticatedUserMessageParsed
        ) where TMessageData : IBaseMessageData;

        /// <summary>
        /// Adds a new automatic user message parser that is fatal on validation fail or error
        /// </summary>
        /// <typeparam name="TMessageData">Message data type</typeparam>
        /// <param name="onPeerMessageParsed">Gets invoked when a peer message has been parsed</param>
        /// <returns>Message parser</returns>
        IUserMessageParser<TAuthenticableUser, TMessageData>
            AddNewAutomaticAuthenticatedUserMessageParserWithFatality<TMessageData>
        (
            UserMessageParsedDelegate<TAuthenticableUser, TMessageData> onPeerMessageParsed
        ) where TMessageData : IBaseMessageData;
    }
}
