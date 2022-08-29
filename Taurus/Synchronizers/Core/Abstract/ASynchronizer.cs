using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Taurus.Connectors;
using Taurus.GUIDs;
using Taurus.Serializers;
using Taurus.Synchronizers.Data.Messages;
using Taurus.Validators;

/// <summary>
/// Taurus synchronizers namespace
/// </summary>
namespace Taurus.Synchronizers
{
    /// <summary>
    /// An abstract class that describes a synchronizer
    /// </summary>
    /// <typeparam name="TUser">User type</typeparam>
    public abstract class ASynchronizer<TUser> : ISynchronizer<TUser> where TUser : IUser
    {
        /// <summary>
        /// Available connectors
        /// </summary>
        private readonly List<IConnector> connectors = new List<IConnector>();

        /// <summary>
        /// User connected events
        /// </summary>
        private readonly ConcurrentQueue<TUser> userConnectedEvents = new ConcurrentQueue<TUser>();

        /// <summary>
        /// Users
        /// </summary>
        private readonly ConcurrentDictionary<UserGUID, TUser> users = new ConcurrentDictionary<UserGUID, TUser>();

        /// <summary>
        /// Peer GUID to user lookup
        /// </summary>
        private readonly ConcurrentDictionary<PeerGUID, TUser> peerGUIDToUserLookup = new ConcurrentDictionary<PeerGUID, TUser>();

        /// <summary>
        /// Available message parsers
        /// </summary>
        private readonly Dictionary<string, List<IBaseUserMessageParser<TUser>>> userMessageParsers = new Dictionary<string, List<IBaseUserMessageParser<TUser>>>();

        /// <summary>
        /// Available connectors
        /// </summary>
        public IEnumerable<IConnector> Connectors => connectors;

        /// <summary>
        /// Serializer
        /// </summary>
        public ISerializer Serializer { get; }

        /// <summary>
        /// Users
        /// </summary>
        public IReadOnlyDictionary<UserGUID, TUser> Users => users;

        /// <summary>
        /// Gets invoked when a peer connection has been attempted
        /// </summary>
        public event PeerConnectionAttemptedDelegate? OnPeerConnectionAttempted;

        /// <summary>
        /// Gets invoked when a peer has been connected
        /// </summary>
        public event PeerConnectedDelegate? OnPeerConnected;

        /// <summary>
        /// Gets invoked when a peer has been disconnected
        /// </summary>
        public event PeerDisconnectedDelegate? OnPeerDisconnected;

        /// <summary>
        /// Gets invoked when a peer message has been received
        /// </summary>
        public event PeerMessageReceivedDelegate? OnPeerMessageReceived;

        /// <summary>
        /// Gets invoked when an user has been connected
        /// </summary>
        public event UserConnectedDelegate<TUser>? OnUserConnected;

        /// <summary>
        /// Gets invoked when an user has been disconnected
        /// </summary>
        public event UserDisconnectedDelegate<TUser>? OnUserDisconnected;

        /// <summary>
        /// Gets invoked when an unknown user message has been received
        /// </summary>
        public event UnknownUserMessageReceivedDelegate<TUser>? OnUnknownUserMessageReceived;

        /// <summary>
        /// Gets invoked when an user error message has been received
        /// </summary>
        public event UserErrorMessageReceivedDelegate<TUser>? OnUserErrorMessageReceived;

        /// <summary>
        /// Gets invoked when an user ping message has been received
        /// </summary>
        public event UserPingMessageReceivedDelegate<TUser>? OnUserPingMessageReceived;

        /// <summary>
        /// Gets invoked when an user pong message has been received
        /// </summary>
        public event UserPongMessageReceivedDelegate<TUser>? OnUserPongMessageReceived;

        /// <summary>
        /// Constructs a generalised synchronizer object
        /// </summary>
        /// <param name="serializer">Serializer</param>
        public ASynchronizer(ISerializer serializer)
        {
            Serializer = serializer;
            AddNewUserMessageParser<ErrorMessageData>
            (
                (user, message, _) =>
                {
                    string error_message = message.Message ?? string.Empty;
                    OnUserErrorMessageReceived?.Invoke(user, message.ErrorType, message.IssuingMessageType ?? string.Empty, error_message);
                    Console.Error.WriteLine($"[{ message.ErrorType }]{ ((message.IssuingMessageType == null) ? $"[{ message.IssuingMessageType }] " : string.Empty) }{ error_message }");
                    return Task.CompletedTask;
                },
                (user, message, _) =>
                {
                    Task ret;
                    if (message.ErrorType == EErrorType.Invalid)
                    {
                        ret = user.SendErrorMessageAsync<ErrorMessageData>
                        (
                            EErrorType.InvalidErrorType,
                            "An error message with an invalid error type has been sent."
                        );
                    }
                    else if (string.IsNullOrWhiteSpace(message.MessageType))
                    {
                        ret = user.SendErrorMessageAsync<ErrorMessageData>
                        (
                            EErrorType.InvalidMessageType,
                            "An error message with an invalid message type has been sent."
                        );
                    }
                    else if (message.Message == null)
                    {
                        ret = user.SendErrorMessageAsync<ErrorMessageData>
                        (
                            EErrorType.MessageIsNull,
                            "An error message with message being null has been sent."
                        );
                    }
                    else
                    {
                        ret = user.SendErrorMessageAsync<ErrorMessageData>(EErrorType.UnknownMessage, "Message validation has failed.");
                    }
                    return ret;
                },
                UserMessageParseFailedEvent<ErrorMessageData>
            );
            AddNewAutomaticUserMessageParser<PingMessageData>
            (
                (user, message, _) =>
                {
                    int key = message.Key!.Value;
                    OnUserPingMessageReceived?.Invoke(user, key);
                    return user.SendMessageAsync(new PongMessageData(key));
                }
            );
            AddNewAutomaticUserMessageParser<PongMessageData>
            (
                (user, message, _) =>
                {
                    Task ret = Task.CompletedTask;
                    int key = message.Key!.Value;
                    if (user.ReceivePongMessageKey(key))
                    {
                        OnUserPongMessageReceived?.Invoke(user, key, user.Latency);
                    }
                    else
                    {
                        ret = user.SendErrorMessageAsync<PongMessageData>(EErrorType.InvalidMessageContext, "No ping message has been sent yet.");
                    }
                    return ret;
                }
            );
        }

        /// <summary>
        /// Adds a new user from the specified peer asynchronously
        /// </summary>
        /// <param name="peer"></param>
        /// <returns></returns>
        /// <exception cref="DuplicateGUIDException{UserGUID}"></exception>
        private async Task<TUser> AddNewUserFromPeerAsync(IPeer peer)
        {
            TUser user = await CreateNewUserAsync(peer);
            bool ret = users.TryAdd(user.UserGUID, user);
            if (!ret)
            {
                throw new DuplicateGUIDException<UserGUID>(user.UserGUID);
            }
            ret = peerGUIDToUserLookup.TryAdd(peer.PeerGUID, user);
            if (!ret)
            {
                users.TryRemove(user.UserGUID, out _);
                throw new DuplicateGUIDException<PeerGUID>(peer.PeerGUID);
            }
            userConnectedEvents.Enqueue(user);
            return user;
        }

        /// <summary>
        /// Listens to any user message parse failed event
        /// </summary>
        /// <typeparam name="TMessageData">Message data type</typeparam>
        /// <param name="user">User</param>
        /// <param name="expectedMessageType">Expected message type</param>
        /// <param name="bytes">Message bytes</param>
        /// <param name="isFatal">Is error fatal</param>
        /// <returns>Task</returns>
        protected Task UserMessageParseFailedEvent<TMessageData>(TUser user, string expectedMessageType, ReadOnlyMemory<byte> bytes, bool isFatal)
            where TMessageData : IBaseMessageData =>
            user.SendErrorMessageAsync<TMessageData>
            (
                EErrorType.InvalidMessageParameters,
                $"Message is invalid. Expected message type: \"{ expectedMessageType }\"{ Environment.NewLine }{ Environment.NewLine }Bytes:{ Environment.NewLine }{ Convert.ToBase64String(bytes.Span) }",
                isFatal
            );

        /// <summary>
        /// Listens to any user message parse failed event
        /// </summary>
        /// <typeparam name="TMessageData">Message data type</typeparam>
        /// <param name="user">User</param>
        /// <param name="expectedMessageType">Expected message tyoe</param>
        /// <param name="bytes">Bytes</param>
        /// <returns>Task</returns>
        protected Task UserMessageParseFailedEvent<TMessageData>(TUser user, string expectedMessageType, ReadOnlyMemory<byte> bytes)
            where TMessageData : IBaseMessageData =>
            UserMessageParseFailedEvent<TMessageData>(user, expectedMessageType, bytes, false);

        /// <summary>
        /// Listens to any user message parse failed event that is fatal
        /// </summary>
        /// <typeparam name="TMessageData">Message data type</typeparam>
        /// <param name="user">User</param>
        /// <param name="expectedMessageType">Expected message type</param>
        /// <param name="bytes">Message bytes</param>
        /// <returns>Task</returns>
        protected Task FatalUserMessageParseFailedEvent<TMessageData>(TUser user, string expectedMessageType, ReadOnlyMemory<byte> bytes)
            where TMessageData : IBaseMessageData =>
            UserMessageParseFailedEvent<TMessageData>(user, expectedMessageType, bytes, true);

        /// <summary>
        /// Listens to any user message validation event
        /// </summary>
        /// <typeparam name="TMessageData">Message data type</typeparam>
        /// <param name="user">User</param>
        /// <param name="message">Received message</param>
        /// <param name="bytes">Message bytes</param>
        /// <param name="isFatal">Is validation fail fatal</param>
        /// <returns>Task</returns>
        protected Task UserMessageValidationFailedEvent<TMessageData>(TUser user, TMessageData message, ReadOnlyMemory<byte> bytes, bool isFatal)
            where TMessageData : IBaseMessageData =>
            user.SendErrorMessageAsync<TMessageData>
            (
                EErrorType.InvalidMessageParameters,
                $"Message is invalid. Message type: \"{ message.GetType().FullName }\"{ Environment.NewLine }{ Environment.NewLine }Bytes:{ Environment.NewLine }{ Convert.ToBase64String(bytes.Span) }",
                isFatal
            );

        /// <summary>
        /// Listens to any user message validation event
        /// </summary>
        /// <typeparam name="TMessageData">Message data type</typeparam>
        /// <param name="user">User</param>
        /// <param name="message">Received message</param>
        /// <param name="bytes">Message bytes</param>
        /// <returns>Task</returns>
        protected Task UserMessageValidationFailedEvent<TMessageData>(TUser user, TMessageData message, ReadOnlyMemory<byte> bytes)
            where TMessageData : IBaseMessageData =>
            UserMessageValidationFailedEvent(user, message, bytes, false);

        /// <summary>
        /// Listens to any user message validation event that is fatal
        /// </summary>
        /// <typeparam name="TMessageData">Message data type</typeparam>
        /// <param name="user">User</param>
        /// <param name="message">Received message</param>
        /// <param name="bytes">Message bytes</param>
        /// <returns>Task</returns>
        protected Task FatalUserMessageValidationFailedEvent<TMessageData>(TUser user, TMessageData message, ReadOnlyMemory<byte> bytes)
            where TMessageData : IBaseMessageData =>
            UserMessageValidationFailedEvent(user, message, bytes, true);

        /// <summary>
        /// Creates a new user from the specified peer asynchronously
        /// </summary>
        /// <param name="peer">Peer</param>
        /// <returns>New user task</returns>
        protected abstract Task<TUser> CreateNewUserAsync(IPeer peer);

        /// <summary>
        /// Adds the specified connector
        /// </summary>
        /// <param name="connector">Connector</param>
        /// <returns>"true" if the specified connector was successfully added, otherwise "false"</returns>
        public bool AddConnector(IConnector connector)
        {
            bool ret = !connectors.Contains(connector);
            if (ret)
            {
                connectors.Add(connector);
                foreach (IPeer peer in connector.Peers.Values)
                {
                    OnPeerConnected?.Invoke(peer);
                    _ = AddNewUserFromPeerAsync(peer);
                }
                connector.OnPeerConnectionAttempted += (peer) => OnPeerConnectionAttempted?.Invoke(peer);
                connector.OnPeerConnected +=
                    (peer) =>
                    {
                        OnPeerConnected?.Invoke(peer);
                        _ = AddNewUserFromPeerAsync(peer);
                    };
                connector.OnPeerDisconnected +=
                    (peer, disconnectionReason) =>
                    {
                        if (TryGettingUserFromPeer(peer, out TUser user))
                        {
                            OnUserDisconnected?.Invoke(user, disconnectionReason);
                            peerGUIDToUserLookup.TryRemove(peer.PeerGUID, out _);
                            users.TryRemove(user.UserGUID, out _);
                        }
                        OnPeerDisconnected?.Invoke(peer, disconnectionReason);
                    };
                connector.OnPeerMessageReceived += (peer, message) =>
                {
                    OnPeerMessageReceived?.Invoke(peer, message);
                    if (TryGettingUserFromPeer(peer, out TUser user))
                    {
                        BaseMessageData? base_network_message_data = Serializer.Deserialize<BaseMessageData?>(message);
                        if (Validator.IsValid(base_network_message_data))
                        {
                            if
                            (
                                userMessageParsers.TryGetValue
                                (
                                    base_network_message_data!.MessageType!,
                                    out List<IBaseUserMessageParser<TUser>> user_message_parsers
                                )
                            )
                            {
                                foreach (IBaseUserMessageParser<TUser> user_message_parser in user_message_parsers)
                                {
                                    user_message_parser.ParseUserMessage(user, message);
                                }
                            }
                            else
                            {
                                OnUnknownUserMessageReceived?.Invoke(user, base_network_message_data, message);
                            }
                        }
                    }
                };
            }
            return ret;
        }

        /// <summary>
        /// Removes the specified connector
        /// </summary>
        /// <param name="connector">Connector</param>
        /// <returns>"true" if the specified connector was successfully removed, otherwise "false"</returns>
        public bool RemoveConnector(IConnector connector)
        {
            bool ret = connectors.Remove(connector);
            if (ret)
            {
                foreach (IPeer peer in connector.Peers.Values)
                {
                    if (TryGettingUserFromPeer(peer, out TUser user))
                    {
                        users.TryRemove(user.UserGUID, out _);
                        peerGUIDToUserLookup.TryRemove(peer.PeerGUID, out _);
                        OnUserDisconnected?.Invoke(user, EDisconnectionReason.Disposed);
                    }
                    OnPeerDisconnected?.Invoke(peer, EDisconnectionReason.Disposed);
                }
            }
            return ret;
        }

        /// <summary>
        /// Gets a connector with the specified type
        /// </summary>
        /// <typeparam name="TConnector">Connector type</typeparam>
        /// <returns>Connector of specified type if successful, otherwise "null"</returns>
        public TConnector GetConnectorOfType<TConnector>() where TConnector : IConnector?
        {
            TryGettingConnectorOfType(out TConnector ret);
            return ret;
        }

        /// <summary>
        /// Tries to get a connector of the specified type
        /// </summary>
        /// <typeparam name="TConnector">Connector type</typeparam>
        /// <param name="connector">Connector</param>
        /// <returns>"true" if connector of the specified type is available, otherwise "false"</returns>
        public bool TryGettingConnectorOfType<TConnector>(out TConnector connector) where TConnector : IConnector?
        {
            bool ret = false;
            connector = default!;
            foreach (IConnector available_connector in connectors)
            {
                if (available_connector is TConnector result)
                {
                    connector = result;
                    ret = true;
                    break;
                }
            }
            return ret;
        }

        /// <summary>
        /// Tries to get user from the specified peer
        /// </summary>
        /// <param name="peer">Peer</param>
        /// <param name="user">User</param>
        /// <returns>"true" if peer is an user, otherwise "false"</returns>
        public bool TryGettingUserFromPeer(IPeer peer, out TUser user) => peerGUIDToUserLookup.TryGetValue(peer.PeerGUID, out user) && (user.Peer == peer);

        /// <summary>
        /// Asserts that the specified peer is an user
        /// </summary>
        /// <param name="peer">Peer</param>
        /// <param name="onPeerIsAnUserAsserted">Gets invoked when the specified peer is an user</param>
        public void AssertPeerIsAnUser(IPeer peer, PeerIsAnUserAssertedDelegate<TUser> onPeerIsAnUserAsserted)
        {
            if (TryGettingUserFromPeer(peer, out TUser user))
            {
                onPeerIsAnUserAsserted(user);
            }
        }

        /// <summary>
        /// Adds a new user message parser
        /// </summary>
        /// <typeparam name="TMessageData">Message data type</typeparam>
        /// <param name="onUserMessageParsed">Gets invoked when a peer message has been parsed</param>
        /// <param name="onUserMessageValidationFailed">Gets invoked when validating a peer message has failed</param>
        /// <param name="onUserMessageParseFailed">Gets invoked when parsing a peer message has failed</param>
        /// <returns>Message parser</returns>
        public IUserMessageParser<TUser, TMessageData> AddNewUserMessageParser<TMessageData>
        (
            UserMessageParsedDelegate<TUser, TMessageData> onUserMessageParsed,
            UserMessageValidationFailedDelegate<TUser, TMessageData> onUserMessageValidationFailed,
            UserMessageParseFailedDelegate<TUser> onUserMessageParseFailed
        ) where TMessageData : IBaseMessageData
        {
            IUserMessageParser<TUser, TMessageData> ret = new UserMessageParser<TUser, TMessageData>(Serializer, onUserMessageParsed, onUserMessageValidationFailed, onUserMessageParseFailed);
            if (!userMessageParsers.TryGetValue(ret.MessageType, out List<IBaseUserMessageParser<TUser>> message_parsers))
            {
                message_parsers = new List<IBaseUserMessageParser<TUser>>();
                userMessageParsers.Add(ret.MessageType, message_parsers);
            }
            message_parsers.Add(ret);
            return ret;
        }

        /// <summary>
        /// Adds a new automatic user message parser
        /// </summary>
        /// <typeparam name="TMessageData">Message data type</typeparam>
        /// <param name="onUserMessageParsed">Gets invoked when an user message has been parsed</param>
        /// <param name="isFatal">Is validation fail or error fatal</param>
        /// <returns>Message parser</returns>
        public IUserMessageParser<TUser, TMessageData> AddNewAutomaticUserMessageParser<TMessageData>
        (
            UserMessageParsedDelegate<TUser, TMessageData> onUserMessageParsed,
            bool isFatal
        ) where TMessageData : IBaseMessageData =>
            AddNewUserMessageParser
            (
                onUserMessageParsed,
                isFatal ?
                    (UserMessageValidationFailedDelegate<TUser, TMessageData>)FatalUserMessageValidationFailedEvent :
                    UserMessageValidationFailedEvent,
                isFatal ?
                    (UserMessageParseFailedDelegate<TUser>)FatalUserMessageParseFailedEvent<TMessageData> :
                    UserMessageParseFailedEvent<TMessageData>
            );

        /// <summary>
        /// Adds a new automatic user message parser
        /// </summary>
        /// <typeparam name="TMessageData">Message data type</typeparam>
        /// <param name="onUserMessageParsed">Gets invoked when an user message has been parsed</param>
        /// <returns>Message parser</returns>
        public IUserMessageParser<TUser, TMessageData> AddNewAutomaticUserMessageParser<TMessageData>
        (
            UserMessageParsedDelegate<TUser, TMessageData> onUserMessageParsed
        ) where TMessageData : IBaseMessageData =>
            AddNewAutomaticUserMessageParser(onUserMessageParsed, false);

        /// <summary>
        /// Adds a new automatic user message parser that is fatal on validation fail or error
        /// </summary>
        /// <typeparam name="TMessageData">Message data type</typeparam>
        /// <param name="onUserMessageParsed">Gets invoked when an user message has been parsed</param>
        /// <returns>Message parser</returns>
        public IUserMessageParser<TUser, TMessageData> AddNewAutomaticUserMessageParserWithFatality<TMessageData>
        (
            UserMessageParsedDelegate<TUser, TMessageData> onUserMessageParsed
        ) where TMessageData : IBaseMessageData =>
            AddNewAutomaticUserMessageParser(onUserMessageParsed, true);

        /// <summary>
        /// Gets user message parsers for the specified type
        /// </summary>
        /// <typeparam name="TMessageData">Message data type</typeparam>
        /// <returns>Message parsers if successful, otherwise "null"</returns>
        public IEnumerable<IUserMessageParser<TUser, TMessageData>> GetUserMessageParsersForType<TMessageData>() where TMessageData : IBaseMessageData =>
            TryGettingUserMessageParsersForType(out IEnumerable<IUserMessageParser<TUser, TMessageData>>? ret) ?
                ret! :
                Array.Empty<IUserMessageParser<TUser, TMessageData>>();

        /// <summary>
        /// Tries to get user message parsers for the specified type
        /// </summary>
        /// <typeparam name="TMessageData">Message data type</typeparam>
        /// <param name="userMessageParsers">User message parsers</param>
        /// <returns>"true" if message parsers are available, otherwise "false"</returns>
        public bool TryGettingUserMessageParsersForType<TMessageData>(out IEnumerable<IUserMessageParser<TUser, TMessageData>>? userMessageParsers)
            where TMessageData : IBaseMessageData
        {
            string key = Naming.GetMessageTypeNameFromMessageDataType<TMessageData>();
            bool ret = this.userMessageParsers.TryGetValue(key, out List<IBaseUserMessageParser<TUser>> message_parsers);
            if (ret)
            {
                List<IUserMessageParser<TUser, TMessageData>> message_parser_list = new List<IUserMessageParser<TUser, TMessageData>>();
                foreach (IBaseUserMessageParser<TUser> base_message_parser in message_parsers)
                {
                    if (base_message_parser is IUserMessageParser<TUser, TMessageData> message_parser)
                    {
                        message_parser_list.Add(message_parser);
                    }
                }
                userMessageParsers = message_parser_list;
            }
            else
            {
                userMessageParsers = null;
            }
            return ret;
        }

        /// <summary>
        /// Removes the specified user message parser
        /// </summary>
        /// <typeparam name="TMessageData">Message data type</typeparam>
        /// <param name="userMessageParser">User message parser</param>
        /// <returns>"true" if message parser was successfully removed, otherwise "false"</returns>
        public bool RemoveUserMessageParser<TMessageData>(IUserMessageParser<TUser, TMessageData> userMessageParser) where TMessageData : IBaseMessageData
        {
            if (userMessageParser == null)
            {
                throw new ArgumentNullException(nameof(userMessageParser));
            }
            bool ret = false;
            if (userMessageParsers.TryGetValue(userMessageParser.MessageType, out List<IBaseUserMessageParser<TUser>> message_parsers))
            {
                ret = message_parsers.Remove(userMessageParser);
                if (ret && (message_parsers.Count <= 0))
                {
                    userMessageParsers.Remove(userMessageParser.MessageType);
                }
            }
            return ret;
        }

        /// <summary>
        /// Closes connections to all peers
        /// </summary>
        /// <param name="reason">Disconnection reason</param>
        public virtual void Close(EDisconnectionReason reason)
        {
            foreach (IConnector connector in connectors)
            {
                connector.Close(reason);
            }
            connectors.Clear();
        }

        /// <summary>
        /// Processes all events appeared since last call
        /// </summary>
        public virtual void ProcessEvents()
        {
            if (OnUserConnected == null)
            {
                userConnectedEvents.Clear();
            }
            else
            {
                while (userConnectedEvents.TryDequeue(out TUser user))
                {
                    OnUserConnected(user);
                }
            }
            foreach (IConnector connector in connectors)
            {
                connector.ProcessEvents();
            }
        }

        /// <summary>
        /// Disposes this object
        /// </summary>
        public void Dispose() => Close(EDisconnectionReason.Disposed);
    }
}
