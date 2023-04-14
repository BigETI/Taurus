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
    /// <typeparam name="TAuthenticatedUserInformation">Authenticated user information type</typeparam>
    /// <typeparam name="TAuthenticationSucceededMessageData">Authentication succeeded message data type</typeparam>
    /// <typeparam name="TAuthenticationFailedMessageData">Authentication failed message data type</typeparam>
    public abstract class AAuthenticableUsersSynchronizer
    <
        TAuthenticableUser,
        TAuthenticateMessageData,
        TAuthenticatedUserInformation,
        TAuthenticationSucceededMessageData,
        TAuthenticationFailedMessageData
    > :
        ASynchronizer<TAuthenticableUser>,
        IAuthenticableUsersSynchronizer<TAuthenticableUser>
        where TAuthenticableUser : IAuthenticableUser
        where TAuthenticateMessageData : IBaseMessageData
        where TAuthenticatedUserInformation : class
        where TAuthenticationSucceededMessageData : class, IBaseMessageData
        where TAuthenticationFailedMessageData : class, IBaseMessageData
    {
        /// <summary>
        /// User authenticated events
        /// </summary>
        private readonly ConcurrentQueue<(TAuthenticableUser AuthenticableUser, TAuthenticatedUserInformation AuthenticatedUserInformation)> userAuthenticatedEvents =
            new ConcurrentQueue<(TAuthenticableUser AuthenticableUser, TAuthenticatedUserInformation AuthenticatedUserInformation)>();

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
        protected AAuthenticableUsersSynchronizer(ISerializer serializer) : base(serializer)
        {
            AddNewAutomaticUserMessageParserWithFatality<TAuthenticateMessageData>
            (
                async (authenticableUser, message, _) =>
                {
                    IUserAuthenticationResult<TAuthenticatedUserInformation, TAuthenticationSucceededMessageData, TAuthenticationFailedMessageData> user_authentication_result =
                        await HandlePeerUserAuthenticationAsync(authenticableUser, message);
                    if (user_authentication_result.IsSuccessful)
                    {
                        userAuthenticatedEvents.Enqueue((authenticableUser, user_authentication_result.AuthenticatedUserInformation!));
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
        /// <param name="authenticateMessageData">Authenticate message data</param>
        /// <returns>User authentication result as a task</returns>
        protected abstract Task<IUserAuthenticationResult<TAuthenticatedUserInformation, TAuthenticationSucceededMessageData, TAuthenticationFailedMessageData>>
            HandlePeerUserAuthenticationAsync(TAuthenticableUser authenticableUser, TAuthenticateMessageData authenticateMessageData);

        /// <summary>
        /// Updates the specified authenticated user
        /// </summary>
        /// <param name="authenticatedUser">Authenticated user</param>
        /// <param name="authenticatedUserInformation">Authenticated user information</param>
        protected abstract void UpdateAuthenticatedUser
        (
            TAuthenticableUser authenticatedUser,
            TAuthenticatedUserInformation authenticatedUserInformation
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
            authenticableUser.SendErrorMessageAsync<TMessageData>(EErrorType.MessageSenderIsNotAuthenticated, $"Message sender sending \"{Naming.GetMessageTypeNameFromMessageDataType<TMessageData>()}\" message is not authenticated.", isFatal);

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
            while (userAuthenticatedEvents.TryDequeue(out (TAuthenticableUser AuthenticableUser, TAuthenticatedUserInformation AuthenticatedUserInformation) user))
            {
                if (!user.AuthenticableUser.IsAuthenticated)
                {
                    user.AuthenticableUser.FlagAsAuthenticated();
                    UpdateAuthenticatedUser(user.AuthenticableUser, user.AuthenticatedUserInformation);
                    OnUserAuthenticated?.Invoke(user.AuthenticableUser);
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
        /// <param name="onUserIsNotAuthenticated">Gets invoked when user is not authenticated</param>
        /// <param name="onAuthenticatedUserMessageValidationFailed">Gets invoked when authenticated user message validation has failed</param>
        /// <param name="onAuthenticatedUserMessageParseFailed">Gets invoked when parsing an authenticated user message has failed</param>
        /// <returns>Peer message parser</returns>
        public IUserMessageParser<TAuthenticableUser, TMessageData> AddNewAuthenticatedUserMessageParser<TMessageData>
        (
            UserMessageParsedDelegate<TAuthenticableUser, TMessageData> onAuthenticatedUserMessageParsed,
            UserMessageValidationFailedDelegate<TAuthenticableUser, TMessageData>
                onAuthenticatedUserMessageValidationFailed,
            UserMessageParseFailedDelegate<TAuthenticableUser> onAuthenticatedUserMessageParseFailed,
            UserIsNotAuthenticatedDelegate<TAuthenticableUser> onUserIsNotAuthenticated
        ) where TMessageData : IBaseMessageData =>
            AddNewUserMessageParser<TMessageData>
            (
                (user, message, bytes) => user.IsAuthenticated ? onAuthenticatedUserMessageParsed(user, message, bytes) : onUserIsNotAuthenticated(user),
                (user, message, bytes) =>
                    user.IsAuthenticated ? onAuthenticatedUserMessageValidationFailed(user, message, bytes) : onUserIsNotAuthenticated(user),
                (user, expectedMessageType, bytes) =>
                    user.IsAuthenticated ? onAuthenticatedUserMessageParseFailed(user, expectedMessageType, bytes) : onUserIsNotAuthenticated(user)
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
        /// <param name="onAuthenticatedUserMessageParsed">Gets invoked when an authenticated user message has been parsed</param>
        /// <returns>Message parser</returns>
        public IUserMessageParser<TAuthenticableUser, TMessageData> AddNewAutomaticAuthenticatedUserMessageParserWithFatality<TMessageData>
        (
            UserMessageParsedDelegate<TAuthenticableUser, TMessageData> onAuthenticatedUserMessageParsed
        ) where TMessageData : IBaseMessageData =>
            AddNewAutomaticAuthenticatedUserMessageParser(onAuthenticatedUserMessageParsed, true);
    }
}
