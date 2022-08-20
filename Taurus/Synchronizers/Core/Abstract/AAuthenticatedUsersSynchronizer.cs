using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Taurus.Connectors;
using Taurus.Serializers;

/// <summary>
/// Taurus synchronizers namespace
/// </summary>
namespace Taurus.Synchronizers
{
    /// <summary>
    /// An abstract class that describes 
    /// </summary>
    /// <typeparam name="TUser">User type</typeparam>
    /// <typeparam name="TAuthenticateMessageData">Authenticate message data type</typeparam>
    /// <typeparam name="TAuthenticationSuccessfulMessageData">Authentication succesful message data type</typeparam>
    /// <typeparam name="TAuthenticationFailedMessageData">Authentication failed message data type</typeparam>
    public abstract class AAuthenticatedUsersSynchronizer
    <
        TUser,
        TAuthenticateMessageData,
        TAuthenticationSuccessfulMessageData,
        TAuthenticationFailedMessageData
    > :
        ASynchronizer,
        IAuthenticatedUsersSynchronizer<TUser>
        where TUser : class, IUser
        where TAuthenticateMessageData : class, IBaseMessageData
        where TAuthenticationSuccessfulMessageData : class, IBaseMessageData
        where TAuthenticationFailedMessageData : class, IBaseMessageData
    {
        /// <summary>
        /// Users
        /// </summary>
        private readonly Dictionary<UserGUID, TUser> users = new Dictionary<UserGUID, TUser>();

        /// <summary>
        /// Peer GUID to user lookup
        /// </summary>
        private readonly Dictionary<PeerGUID, TUser> peerGUIDToUserLookup = new Dictionary<PeerGUID, TUser>();

        /// <summary>
        /// User authenticated events
        /// </summary>
        private readonly ConcurrentQueue<TUser> userAuthenticatedEvents = new ConcurrentQueue<TUser>();

        /// <summary>
        /// Peer user authentication failed events
        /// </summary>
        private readonly ConcurrentQueue<IPeer> peerUserAuthenticationFailedEvents = new ConcurrentQueue<IPeer>();

        /// <summary>
        /// Users
        /// </summary>
        public IReadOnlyDictionary<UserGUID, TUser> Users => users;

        /// <summary>
        /// Gets invoked when an user has been authenticated
        /// </summary>
        public event UserAuthenticatedDelegate<TUser>? OnUserAuthenticated;

        /// <summary>
        /// Gets invoked when a peer user has been authenticated
        /// </summary>
        public event PeerUserAuthenticationFailedDelegate? OnPeerUserAuthenticationFailed;

        /// <summary>
        /// Gets invoked when an user has been disconnected
        /// </summary>
        public event UserDisconnectedDelegate<TUser>? OnUserDisconnected;

        /// <summary>
        /// Constructs a new authenticated users synchronizer
        /// </summary>
        /// <param name="serializer">Serializer</param>
        public AAuthenticatedUsersSynchronizer(ISerializer serializer) : base(serializer)
        {
            OnPeerDisconnected +=
                (peer, disconnectionReason) =>
                {
                    if (TryGettingUserFromPeer(peer, out TUser user))
                    {
                        OnUserDisconnected?.Invoke(user, disconnectionReason);
                        users.Remove(user.UserGUID);
                    }
                    peerGUIDToUserLookup.Remove(peer.PeerGUID);
                };
            AddAutomaticPeerMessageParserWithFatality<TAuthenticateMessageData>
            (
                async (peer, message, _) =>
                {
                    IUserAuthenticationResult<TUser, TAuthenticationSuccessfulMessageData, TAuthenticationFailedMessageData> authentication_user_result = await HandlePeerUserAuthenticationAsync(peer, message);
                    if (authentication_user_result.IsSuccessful)
                    {
                        userAuthenticatedEvents.Enqueue(authentication_user_result.User!);
                        await SendMessageToPeerAsync(peer, authentication_user_result.AuthenticationSuccessfulMessageData!);
                    }
                    else
                    {
                        peerUserAuthenticationFailedEvents.Enqueue(peer);
                        await SendMessageToPeerAsync(peer, authentication_user_result.AuthenticationFailedMessageData!);
                    }
                }
            );
        }

        /// <summary>
        /// Handles peer user authentication asynchronously
        /// </summary>
        /// <param name="peer">Peer</param>
        /// <param name="authenticationMessageData">Authentication message data</param>
        /// <returns>User if successful, otherwise "null" as a task</returns>
        protected abstract Task<IUserAuthenticationResult<TUser, TAuthenticationSuccessfulMessageData, TAuthenticationFailedMessageData>> HandlePeerUserAuthenticationAsync
        (
            IPeer peer,
            TAuthenticateMessageData authenticationMessageData
        );

        /// <summary>
        /// Listens to any peer is not authenticated event
        /// </summary>
        /// <typeparam name="TMessageData">Message data type</typeparam>
        /// <param name="peer">Peer</param>
        /// <param name="isFatal">Is fatal</param>
        /// <returns>Task</returns>
        protected Task PeerIsNotAuthenticatedEvent<TMessageData>(IPeer peer, bool isFatal)
            where TMessageData : IBaseMessageData =>
            SendErrorMessageToPeerAsync<TMessageData>(peer, EErrorType.MessageSenderIsNotAuthenticated, $"Message sender sending \"{ Naming.GetMessageTypeNameFromMessageDataType<TMessageData>() }\" message is not authenticated.", isFatal);

        /// <summary>
        /// Listens to any peer is not authenticated event
        /// </summary>
        /// <typeparam name="TMessageData">Message data type</typeparam>
        /// <param name="peer">Peer</param>
        /// <returns>Task</returns>
        protected Task PeerIsNotAuthenticatedEvent<TMessageData>(IPeer peer)
            where TMessageData : IBaseMessageData =>
            PeerIsNotAuthenticatedEvent<TMessageData>(peer, false);

        /// <summary>
        /// Listens to any peer is not authenticated event that is fatal
        /// </summary>
        /// <typeparam name="TMessageData">Message data type</typeparam>
        /// <param name="peer">Peer</param>
        /// <returns>Task</returns>
        protected Task FatalPeerIsNotAuthenticatedEvent<TMessageData>(IPeer peer)
            where TMessageData : IBaseMessageData =>
            PeerIsNotAuthenticatedEvent<TMessageData>(peer, true);

        /// <summary>
        /// Listens to any authenticated user message parse failed event
        /// </summary>
        /// <typeparam name="TMessageData">Message data type</typeparam>
        /// <param name="user">User</param>
        /// <param name="expectedMessageType">Expected message type</param>
        /// <param name="bytes">Message bytes</param>
        /// <param name="isFatal">Is error fatal</param>
        /// <returns>Task</returns>
        protected Task AuthenticatedUserMessageParseFailedEvent<TMessageData>
        (
            TUser user,
            string expectedMessageType,
            ReadOnlyMemory<byte> bytes,
            bool isFatal
        ) where TMessageData : IBaseMessageData =>
            PeerMessageParseFailedEvent<TMessageData>(user.Peer, expectedMessageType, bytes, isFatal);

        /// <summary>
        /// Listens to any authenticated user message parse failed event
        /// </summary>
        /// <typeparam name="TMessageData">Message data type</typeparam>
        /// <param name="user">User</param>
        /// <param name="expectedMessageType">Expected message tyoe</param>
        /// <param name="bytes">Bytes</param>
        /// <returns>Task</returns>
        protected Task AuthenticatedUserMessageParseFailedEvent<TMessageData>
        (
            TUser user,
            string expectedMessageType,
            ReadOnlyMemory<byte> bytes
        ) where TMessageData : IBaseMessageData =>
            PeerMessageParseFailedEvent<TMessageData>(user.Peer, expectedMessageType, bytes);

        /// <summary>
        /// Listens to any authenticated user message parse failed event that is fatal
        /// </summary>
        /// <typeparam name="TMessageData">Message data type</typeparam>
        /// <param name="user">User</param>
        /// <param name="expectedMessageType">Expected message type</param>
        /// <param name="bytes">Message bytes</param>
        /// <returns>Task</returns>
        protected Task FatalAuthenticatedUserMessageParseFailedEvent<TMessageData>(TUser user, string expectedMessageType, ReadOnlyMemory<byte> bytes)
            where TMessageData : IBaseMessageData =>
            FatalPeerMessageParseFailedEvent<TMessageData>(user.Peer, expectedMessageType, bytes);

        /// <summary>
        /// Listens to any authenticated user message validation event
        /// </summary>
        /// <typeparam name="TMessageData">Message data type</typeparam>
        /// <param name="user">User</param>
        /// <param name="message">Received message</param>
        /// <param name="bytes">Message bytes</param>
        /// <param name="isFatal">Is validation fail fatal</param>
        /// <returns>Task</returns>
        protected Task AuthenticatedUserMessageValidationFailedEvent<TMessageData>
        (
            TUser user,
            TMessageData message,
            ReadOnlyMemory<byte> bytes,
            bool isFatal
        ) where TMessageData : IBaseMessageData =>
            PeerMessageValidationFailedEvent(user.Peer, message, bytes, isFatal);

        /// <summary>
        /// Listens to any authenticated user message validation event
        /// </summary>
        /// <typeparam name="TMessageData">Message data type</typeparam>
        /// <param name="user">User</param>
        /// <param name="message">Received message</param>
        /// <param name="bytes">Message bytes</param>
        /// <returns>Task</returns>
        protected Task AuthenticatedUserMessageValidationFailedEvent<TMessageData>(TUser user, TMessageData message, ReadOnlyMemory<byte> bytes)
            where TMessageData : IBaseMessageData =>
            PeerMessageValidationFailedEvent(user.Peer, message, bytes);

        /// <summary>
        /// Listens to any authenticated user message validation event that is fatal
        /// </summary>
        /// <typeparam name="TMessageData">Message data type</typeparam>
        /// <param name="user">User</param>
        /// <param name="message">Received message</param>
        /// <param name="bytes">Message bytes</param>
        /// <returns>Task</returns>
        protected Task FatalAuthenticatedUserMessageValidationFailedEvent<TMessageData>(TUser user, TMessageData message, ReadOnlyMemory<byte> bytes)
            where TMessageData : IBaseMessageData =>
            FatalPeerMessageValidationFailedEvent(user.Peer, message, bytes);

        /// <summary>
        /// Processes all events appeared since last call
        /// </summary>
        public override void ProcessEvents()
        {
            base.ProcessEvents();
            while (userAuthenticatedEvents.TryDequeue(out TUser user))
            {
                if (users.TryAdd(user.UserGUID, user) && peerGUIDToUserLookup.TryAdd(user.Peer.PeerGUID, user))
                {
                    OnUserAuthenticated?.Invoke(user);
                }
            }
            if (OnPeerUserAuthenticationFailed == null)
            {
                peerUserAuthenticationFailedEvents.Clear();
            }
            else
            {
                while (peerUserAuthenticationFailedEvents.TryDequeue(out IPeer peer))
                {
                    OnPeerUserAuthenticationFailed.Invoke(peer);
                }
            }
        }

        /// <summary>
        /// Tries to get user from the specified peer
        /// </summary>
        /// <param name="peer">Peer</param>
        /// <param name="user">User</param>
        /// <returns>"true" if peer is an user, otherwise "false"</returns>
        public bool TryGettingUserFromPeer(IPeer peer, out TUser user) =>
            peerGUIDToUserLookup.TryGetValue(peer.PeerGUID, out user) && (user.Peer == peer);

        /// <summary>
        /// Asserts that the specified peer is an user
        /// </summary>
        /// <param name="peer">Peer</param>
        /// <param name="onPeerIsAnUserAsserted">Gets invoked when the specified peer is an user</param>
        public void AssertPeerIsAnUser(IPeer peer, PeerIsAnUserAssertedDelegate onPeerIsAnUserAsserted)
        {
            if (TryGettingUserFromPeer(peer, out TUser user))
            {
                onPeerIsAnUserAsserted(user);
            }
        }

        /// <summary>
        /// Adds an authenticated user message parser
        /// </summary>
        /// <typeparam name="TMessageData">Message data type</typeparam>
        /// <param name="onAuthenticatedUserMessageParsed">Gets invoked when an authenticated user message has been parsed</param>
        /// <param name="onPeerIsNotAuthenticated">Gets invoked when peer is not authenticated</param>
        /// <param name="onAuthenticatedUserMessageValidationFailed">Gets invoked when authenticated user message validation has failed</param>
        /// <param name="onAuthenticatedUserMessageParseFailed">Gets invoked when parsing an authenticated user message has failed</param>
        /// <returns>Peer message parser</returns>
        public IPeerMessageParser<TMessageData> AddAuthenticatedUserMessageParser<TMessageData>
        (
            AuthenticatedUserMessageParsedDelegate<TUser, TMessageData> onAuthenticatedUserMessageParsed,
            PeerIsNotAuthenticatedDelegate onPeerIsNotAuthenticated,
            AuthenticatedUserMessageValidationFailedDelegate<TUser, TMessageData> onAuthenticatedUserMessageValidationFailed,
            AuthenticatedUserMessageParseFailedDelegate<TUser> onAuthenticatedUserMessageParseFailed
        ) where TMessageData : IBaseMessageData =>
            AddPeerMessageParser<TMessageData>
            (
                (peer, message, bytes) =>
                {
                    Task ret;
                    if (TryGettingUserFromPeer(peer, out TUser user))
                    {
                        ret = onAuthenticatedUserMessageParsed(user, message, bytes);
                    }
                    else
                    {
                        ret = onPeerIsNotAuthenticated(peer);
                    }
                    return ret;
                },
                (peer, message, bytes) =>
                {
                    Task ret;
                    if (TryGettingUserFromPeer(peer, out TUser user))
                    {
                        ret = onAuthenticatedUserMessageValidationFailed(user, message, bytes);
                    }
                    else
                    {
                        ret = onPeerIsNotAuthenticated(peer);
                    }
                    return ret;
                },
                (peer, expectedMessageType, bytes) =>
                {
                    Task ret;
                    if (TryGettingUserFromPeer(peer, out TUser user))
                    {
                        ret = onAuthenticatedUserMessageParseFailed(user, expectedMessageType, bytes);
                    }
                    else
                    {
                        ret = onPeerIsNotAuthenticated(peer);
                    }
                    return ret;
                }
            );

        /// <summary>
        /// Adds an automatic authenticated user message parser
        /// </summary>
        /// <typeparam name="TMessageData">Message data type</typeparam>
        /// <param name="onAuthenticatedUserMessageParsed">Gets invoked when an authenticated user message has been parsed</param>
        /// <param name="isFatal">Is validation fail or error fatal</param>
        /// <returns>Message parser</returns>
        public IPeerMessageParser<TMessageData> AddAutomaticAuthenticatedUserMessageParser<TMessageData>
        (
            AuthenticatedUserMessageParsedDelegate<TUser, TMessageData> onAuthenticatedUserMessageParsed,
            bool isFatal
        ) where TMessageData : IBaseMessageData =>
            AddAuthenticatedUserMessageParser
            (
                onAuthenticatedUserMessageParsed,
                isFatal ?
                    (PeerIsNotAuthenticatedDelegate)FatalPeerIsNotAuthenticatedEvent<TMessageData> :
                    PeerIsNotAuthenticatedEvent<TMessageData>,
                isFatal ?
                    (AuthenticatedUserMessageValidationFailedDelegate<TUser, TMessageData>)FatalAuthenticatedUserMessageValidationFailedEvent :
                    AuthenticatedUserMessageValidationFailedEvent,
                isFatal ?
                    (AuthenticatedUserMessageParseFailedDelegate<TUser>)FatalAuthenticatedUserMessageParseFailedEvent<TMessageData> :
                    AuthenticatedUserMessageParseFailedEvent<TMessageData>
            );

        /// <summary>
        /// Adds an automatic authenticated user message parser
        /// </summary>
        /// <typeparam name="TMessageData">Message data type</typeparam>
        /// <param name="onAuthenticatedUserMessageParsed">Gets invoked when an authenticated user message has been parsed</param>
        /// <returns>Message parser</returns>
        public IPeerMessageParser<TMessageData> AddAutomaticAuthenticatedUserMessageParser<TMessageData>
        (
            AuthenticatedUserMessageParsedDelegate<TUser, TMessageData> onAuthenticatedUserMessageParsed
        ) where TMessageData : IBaseMessageData =>
            AddAutomaticAuthenticatedUserMessageParser(onAuthenticatedUserMessageParsed, false);

        /// <summary>
        /// Adds an automatic authenticated user message parser that is fatal on validation fail or error
        /// </summary>
        /// <typeparam name="TMessageData">Message data type</typeparam>
        /// <param name="onAuthenticatedUserMessageParsed">Gets invoked when an authenticated user message has been parsed</param>
        /// <returns>Message parser</returns>
        public IPeerMessageParser<TMessageData> AddAutomaticAuthenticatedUserMessageParserWithFatality<TMessageData>
        (
            AuthenticatedUserMessageParsedDelegate<TUser, TMessageData> onAuthenticatedUserMessageParsed
        ) where TMessageData : IBaseMessageData =>
            AddAutomaticAuthenticatedUserMessageParser(onAuthenticatedUserMessageParsed, true);
    }
}
