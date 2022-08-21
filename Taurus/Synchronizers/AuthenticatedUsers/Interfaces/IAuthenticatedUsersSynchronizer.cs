using System.Collections.Generic;
using Taurus.Connectors;

/// <summary>
/// Taurus synchronizers authenticated users namespace
/// </summary>
namespace Taurus.Synchronizers.AuthenticatedUsers
{
    /// <summary>
    /// An interface that represents an authenticated users synchronizer
    /// </summary>
    /// <typeparam name="TUser">User type</typeparam>
    public interface IAuthenticatedUsersSynchronizer<TUser> : ISynchronizer where TUser : IUser
    {
        /// <summary>
        /// Users
        /// </summary>
        IReadOnlyDictionary<UserGUID, TUser> Users { get; }

        /// <summary>
        /// Gets invoked when an user has been authenticated
        /// </summary>
        event UserAuthenticatedDelegate<TUser>? OnUserAuthenticated;

        /// <summary>
        /// Gets invoked when a peer user has been authenticated
        /// </summary>
        event PeerUserAuthenticationFailedDelegate? OnPeerUserAuthenticationFailed;

        /// <summary>
        /// Gets invoked when an user has been disconnected
        /// </summary>
        event UserDisconnectedDelegate<TUser>? OnUserDisconnected;

        /// <summary>
        /// Tries to get user from the specified peer
        /// </summary>
        /// <param name="peer">Peer</param>
        /// <param name="user">User</param>
        /// <returns>"true" if peer is an user, otherwise "false"</returns>
        bool TryGettingUserFromPeer(IPeer peer, out TUser user);

        /// <summary>
        /// Asserts that the specified peer is an user
        /// </summary>
        /// <param name="peer">Peer</param>
        /// <param name="onPeerIsAnUserAsserted">Gets invoked when the specified peer is an user</param>
        void AssertPeerIsAnUser(IPeer peer, PeerIsAnUserAssertedDelegate<TUser> onPeerIsAnUserAsserted);

        /// <summary>
        /// Adds an authenticated user message parser
        /// </summary>
        /// <typeparam name="TMessageData">Message data type</typeparam>
        /// <param name="onAuthenticatedUserMessageParsed">Gets invoked when an authenticated user message has been parsed</param>
        /// <param name="onPeerIsNotAuthenticated">Gets invoked when peer is not authenticated</param>
        /// <param name="onAuthenticatedUserMessageValidationFailed">Gets invoked when authenticated user message validation has failed</param>
        /// <param name="onAuthenticatedUserMessageParseFailed">Gets invoked when parsing an authenticated user message has failed</param>
        /// <returns>Peer message parser</returns>
        IPeerMessageParser<TMessageData> AddAuthenticatedUserMessageParser<TMessageData>
        (
            AuthenticatedUserMessageParsedDelegate<TUser, TMessageData> onAuthenticatedUserMessageParsed,
            PeerIsNotAuthenticatedDelegate onPeerIsNotAuthenticated,
            AuthenticatedUserMessageValidationFailedDelegate<TUser, TMessageData> onAuthenticatedUserMessageValidationFailed,
            AuthenticatedUserMessageParseFailedDelegate<TUser> onAuthenticatedUserMessageParseFailed
        ) where TMessageData : IBaseMessageData;

        /// <summary>
        /// Adds an automatic authenticated user message parser
        /// </summary>
        /// <typeparam name="TMessageData">Message data type</typeparam>
        /// <param name="onAuthenticatedUserMessageParsed">Gets invoked when an authenticated user message has been parsed</param>
        /// <param name="isFatal">Is validation fail or error fatal</param>
        /// <returns>Message parser</returns>
        IPeerMessageParser<TMessageData> AddAutomaticAuthenticatedUserMessageParser<TMessageData>
        (
            AuthenticatedUserMessageParsedDelegate<TUser, TMessageData> onAuthenticatedUserMessageParsed,
            bool isFatal
        ) where TMessageData : IBaseMessageData;

        /// <summary>
        /// Adds an automatic authenticated user message parser
        /// </summary>
        /// <typeparam name="TMessageData">Message data type</typeparam>
        /// <param name="onAuthenticatedUserMessageParsed">Gets invoked when an authenticated user message has been parsed</param>
        /// <returns>Message parser</returns>
        IPeerMessageParser<TMessageData> AddAutomaticAuthenticatedUserMessageParser<TMessageData>
        (
            AuthenticatedUserMessageParsedDelegate<TUser, TMessageData> onAuthenticatedUserMessageParsed
        ) where TMessageData : IBaseMessageData;

        /// <summary>
        /// Adds an automatic authenticated user message parser that is fatal on validation fail or error
        /// </summary>
        /// <typeparam name="TMessageData">Message data type</typeparam>
        /// <param name="onAuthenticatedUserMessageParsed">Gets invoked when an authenticated user message has been parsed</param>
        /// <returns>Message parser</returns>
        IPeerMessageParser<TMessageData> AddAutomaticAuthenticatedUserMessageParserWithFatality<TMessageData>
        (
            AuthenticatedUserMessageParsedDelegate<TUser, TMessageData> onAuthenticatedUserMessageParsed
        ) where TMessageData : IBaseMessageData;
    }
}
