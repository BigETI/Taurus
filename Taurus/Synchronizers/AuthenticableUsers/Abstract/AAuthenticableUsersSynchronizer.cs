using System.Collections.Concurrent;
using System.Threading.Tasks;
using Taurus.Serializers;

/// <summary>
/// Taurus synchronizers authenticable users namespace
/// </summary>
namespace Taurus.Synchronizers.AuthenticableUsers
{
    /// <summary>
    /// An abstract class that describes 
    /// </summary>
    /// <typeparam name="TAuthenticableUser">Authenticable user type</typeparam>
    /// <typeparam name="TAuthenticateMessageData">Authenticate message data type</typeparam>
    /// <typeparam name="TAuthenticationSuccessfulMessageData">Authentication succesful message data type</typeparam>
    /// <typeparam name="TAuthenticationFailedMessageData">Authentication failed message data type</typeparam>
    public abstract class AAuthenticableUsersSynchronizer
    <
        TAuthenticableUser,
        TAuthenticateMessageData,
        TAuthenticationSuccessfulMessageData,
        TAuthenticationFailedMessageData
    > :
        ASynchronizer<TAuthenticableUser>,
        IAuthenticableUsersSynchronizer<TAuthenticableUser>
        where TAuthenticableUser : IAuthenticableUser
        where TAuthenticateMessageData : IBaseMessageData
        where TAuthenticationSuccessfulMessageData : class, IBaseMessageData
        where TAuthenticationFailedMessageData : class, IBaseMessageData
    {
        /// <summary>
        /// User authenticated events
        /// </summary>
        private readonly ConcurrentQueue<TAuthenticableUser> userAuthenticatedEvents = new ConcurrentQueue<TAuthenticableUser>();

        /// <summary>
        /// User authentication failed events
        /// </summary>
        private readonly ConcurrentQueue<TAuthenticableUser> userAuthenticationFailedEvents = new ConcurrentQueue<TAuthenticableUser>();

        /// <summary>
        /// Gets invoked when an user has been authenticated
        /// </summary>
        public event UserAuthenticatedDelegate<TAuthenticableUser>? OnUserAuthenticated;

        /// <summary>
        /// Gets invoked when a peer user has been authenticated
        /// </summary>
        public event UserAuthenticationFailedDelegate<TAuthenticableUser>? OnUserAuthenticationFailed;

        /// <summary>
        /// Constructs a new authenticated users synchronizer
        /// </summary>
        /// <param name="serializer">Serializer</param>
        public AAuthenticableUsersSynchronizer(ISerializer serializer) : base(serializer)
        {
            AddNewAutomaticUserMessageParserWithFatality<TAuthenticateMessageData>
            (
                async (authenticableUser, message, _) =>
                {
                    IUserAuthenticationResult<TAuthenticationSuccessfulMessageData, TAuthenticationFailedMessageData> user_authentication_result =
                        await HandlePeerUserAuthenticationAsync(authenticableUser, message);
                    if (user_authentication_result.IsSuccessful)
                    {
                        userAuthenticatedEvents.Enqueue(authenticableUser);
                        await authenticableUser.SendMessageAsync(user_authentication_result.AuthenticationSuccessfulMessageData!);
                    }
                    else
                    {
                        userAuthenticationFailedEvents.Enqueue(authenticableUser);
                        await authenticableUser.SendMessageAsync(user_authentication_result.AuthenticationFailedMessageData!);
                    }
                }
            );
        }

        /// <summary>
        /// Handles peer user authentication asynchronously
        /// </summary>
        /// <param name="authenticableUser">Authenticable user</param>
        /// <param name="authenticationMessageData">Authentication message data</param>
        /// <returns>User authentication result as a task</returns>
        protected abstract Task<IUserAuthenticationResult<TAuthenticationSuccessfulMessageData, TAuthenticationFailedMessageData>>
            HandlePeerUserAuthenticationAsync(TAuthenticableUser authenticableUser, TAuthenticateMessageData authenticationMessageData);

        /// <summary>
        /// Updates the specified authenticated user
        /// </summary>
        /// <param name="authenticatedUser">Authenticated user</param>
        /// <param name="authenticationSuccessfulMessageData">Authentication successful message data</param>
        protected abstract void UpdateAuthenticatedUser
        (
            TAuthenticableUser authenticatedUser,
            TAuthenticationSuccessfulMessageData authenticationSuccessfulMessageData
        );

        /// <summary>
        /// Listens to any user is not authenticated event
        /// </summary>
        /// <typeparam name="TMessageData">Message data type</typeparam>
        /// <param name="authenticableUser">Authenticable user</param>
        /// <param name="isFatal">Is fatal</param>
        /// <returns>Task</returns>
        protected Task UserIsNotAuthenticatedEvent<TMessageData>(TAuthenticableUser authenticableUser, bool isFatal)
            where TMessageData : IBaseMessageData =>
            authenticableUser.SendErrorMessageAsync<TMessageData>(EErrorType.MessageSenderIsNotAuthenticated, $"Message sender sending \"{ Naming.GetMessageTypeNameFromMessageDataType<TMessageData>() }\" message is not authenticated.", isFatal);

        /// <summary>
        /// Listens to any user is not authenticated event
        /// </summary>
        /// <typeparam name="TMessageData">Message data type</typeparam>
        /// <param name="authenticableUser">Authenticable user</param>
        /// <returns>Task</returns>
        protected Task UserIsNotAuthenticatedEvent<TMessageData>(TAuthenticableUser authenticableUser)
            where TMessageData : IBaseMessageData =>
            UserIsNotAuthenticatedEvent<TMessageData>(authenticableUser, false);

        /// <summary>
        /// Listens to any user is not authenticated event that is fatal
        /// </summary>
        /// <typeparam name="TMessageData">Message data type</typeparam>
        /// <param name="authenticableUser">Authenticable user</param>
        /// <returns>Task</returns>
        protected Task FatalUserIsNotAuthenticatedEvent<TMessageData>(TAuthenticableUser authenticableUser) where TMessageData : IBaseMessageData =>
            UserIsNotAuthenticatedEvent<TMessageData>(authenticableUser, true);

        /// <summary>
        /// Processes all events appeared since last call
        /// </summary>
        public override void ProcessEvents()
        {
            base.ProcessEvents();
            while (userAuthenticatedEvents.TryDequeue(out TAuthenticableUser user))
            {
                if (!user.IsAuthenticated)
                {
                    user.FlagAsAuthenticated();
                    OnUserAuthenticated?.Invoke(user);
                }
            }
            if (OnUserAuthenticationFailed == null)
            {
                userAuthenticationFailedEvents.Clear();
            }
            else
            {
                while (userAuthenticationFailedEvents.TryDequeue(out TAuthenticableUser user))
                {
                    OnUserAuthenticationFailed.Invoke(user);
                }
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
        public IUserMessageParser<TAuthenticableUser, TMessageData> AddNewAuthenticatedUserMessageParser<TMessageData>
        (
            UserMessageParsedDelegate<TAuthenticableUser, TMessageData> onAuthenticatedUserMessageParsed,
            UserMessageValidationFailedDelegate<TAuthenticableUser, TMessageData>
                onAuthenticatedUserMessageValidationFailed,
            UserMessageParseFailedDelegate<TAuthenticableUser> onAuthenticatedUserMessageParseFailed,
            UserIsNotAuthenticatedDelegate<TAuthenticableUser> onPeerIsNotAuthenticated
        ) where TMessageData : IBaseMessageData =>
            AddNewUserMessageParser<TMessageData>
            (
                (user, message, bytes) => user.IsAuthenticated ? onAuthenticatedUserMessageParsed(user, message, bytes) : onPeerIsNotAuthenticated(user),
                (user, message, bytes) =>
                    user.IsAuthenticated ? onAuthenticatedUserMessageValidationFailed(user, message, bytes) : onPeerIsNotAuthenticated(user),
                (user, expectedMessageType, bytes) =>
                    user.IsAuthenticated ? onAuthenticatedUserMessageParseFailed(user, expectedMessageType, bytes) : onPeerIsNotAuthenticated(user)
            );

        /// <summary>
        /// Adds a new automatic user message parser
        /// </summary>
        /// <typeparam name="TMessageData">Message data type</typeparam>
        /// <param name="onAuthenticatedUserMessageParsed">Gets invoked when an authenticated user message has been parsed</param>
        /// <param name="isFatal">Is validation fail or error fatal</param>
        /// <returns>Message parser</returns>
        public IUserMessageParser<TAuthenticableUser, TMessageData> AddNewAutomaticAuthenticatedUserMessageParser<TMessageData>
        (
            UserMessageParsedDelegate<TAuthenticableUser, TMessageData> onAuthenticatedUserMessageParsed,
            bool isFatal
        ) where TMessageData : IBaseMessageData =>
            AddNewAuthenticatedUserMessageParser
            (
                onAuthenticatedUserMessageParsed,
                isFatal ?
                    (UserMessageValidationFailedDelegate<TAuthenticableUser, TMessageData>)
                        FatalUserMessageValidationFailedEvent :
                    UserMessageValidationFailedEvent,
                isFatal ?
                    (UserMessageParseFailedDelegate<TAuthenticableUser>)FatalUserMessageParseFailedEvent<TMessageData> :
                    UserMessageParseFailedEvent<TMessageData>,
                isFatal ?
                    (UserIsNotAuthenticatedDelegate<TAuthenticableUser>)UserIsNotAuthenticatedEvent<TMessageData> :
                    FatalUserIsNotAuthenticatedEvent<TMessageData>
            );

        /// <summary>
        /// Adds a new automatic user message parser
        /// </summary>
        /// <typeparam name="TMessageData">Message data type</typeparam>
        /// <param name="onAuthenticatedUserMessageParsed">Gets invoked when an authenticated user message has been parsed</param>
        /// <returns>Message parser</returns>
        public IUserMessageParser<TAuthenticableUser, TMessageData> AddNewAutomaticAuthenticatedUserMessageParser<TMessageData>
        (
            UserMessageParsedDelegate<TAuthenticableUser, TMessageData> onAuthenticatedUserMessageParsed
        ) where TMessageData : IBaseMessageData =>
            AddNewAutomaticAuthenticatedUserMessageParser(onAuthenticatedUserMessageParsed, false);

        /// <summary>
        /// Adds a new automatic user message parser that is fatal on validation fail or error
        /// </summary>
        /// <typeparam name="TMessageData">Message data type</typeparam>
        /// <param name="onPeerMessageParsed">Gets invoked when a peer message has been parsed</param>
        /// <returns>Message parser</returns>
        public IUserMessageParser<TAuthenticableUser, TMessageData> AddNewAutomaticAuthenticatedUserMessageParserWithFatality<TMessageData>
        (
            UserMessageParsedDelegate<TAuthenticableUser, TMessageData> onPeerMessageParsed
        ) where TMessageData : IBaseMessageData =>
            AddNewAutomaticAuthenticatedUserMessageParser(onPeerMessageParsed, true);
    }
}
