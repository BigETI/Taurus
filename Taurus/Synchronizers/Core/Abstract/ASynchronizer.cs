using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Taurus.Connectors;
using Taurus.Serializers;
using Taurus.Synchronizers.Data.Messages;

/// <summary>
/// Taurus synchronizers namespace
/// </summary>
namespace Taurus.Synchronizers
{
    /// <summary>
    /// A class that describes a synchronizer
    /// </summary>
    /// <typeparam name="TAuthenticationMessageData"></typeparam>
    public abstract class ASynchronizer<TAuthenticationMessageData> : ISynchronizer where TAuthenticationMessageData : IBaseMessageData
    {
        /// <summary>
        /// Available connectors
        /// </summary>
        private readonly List<IConnector> connectors = new List<IConnector>();

        /// <summary>
        /// Available message parsers
        /// </summary>
        private readonly Dictionary<string, List<IBaseMessageParser>> messageParsers = new Dictionary<string, List<IBaseMessageParser>>();

        /// <summary>
        /// Users
        /// </summary>
        private readonly Dictionary<Guid, IUser> users = new Dictionary<Guid, IUser>();

        /// <summary>
        /// Peer GUID to user lookup
        /// </summary>
        private readonly Dictionary<Guid, IUser> peerGUIDToUserLookup = new Dictionary<Guid, IUser>();

        /// <summary>
        /// User authenticated events
        /// </summary>
        private readonly ConcurrentQueue<IUser> userAuthenticatedEvents = new ConcurrentQueue<IUser>();

        /// <summary>
        /// Available connectors
        /// </summary>
        public IEnumerable<IConnector> Connectors => connectors;

        /// <summary>
        /// Users
        /// </summary>
        public IReadOnlyDictionary<Guid, IUser> IUsers => users;

        /// <summary>
        /// Serializer
        /// </summary>
        public ISerializer Serializer { get; }

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
        /// Gets invoked when an unknown peer message has been received
        /// </summary>
        public event UnknownPeerMessageReceivedDelegate? OnUnknownPeerMessageReceived;

        /// <summary>
        /// Gets invoked when an peer error message has been received
        /// </summary>
        public event PeerErrorMessageReceivedDelegate? OnPeerErrorMessageReceived;

        /// <summary>
        /// Gets invoked when an user has been authenticated
        /// </summary>
        public event UserAuthenticatedDelegate? OnUserAuthenticated;

        /// <summary>
        /// Gets invoked when an user has been disconnected
        /// </summary>
        public event UserDisconnectedDelegate? OnUserDisconnected;

        /// <summary>
        /// Constructs a generalised synchronizer object
        /// </summary>
        /// <param name="serializer">Serializer</param>
        public ASynchronizer(ISerializer serializer)
        {
            Serializer = serializer;
            AddMessageParser<ErrorMessageData>
            (
                (_, message, __) =>
                {
                    string error_message = message.Message ?? string.Empty;
                    OnPeerErrorMessageReceived?.Invoke(message.ErrorType, message.IssuingMessageType ?? string.Empty, error_message);
                    Console.Error.WriteLine($"[{ message.ErrorType }]{ ((message.IssuingMessageType == null) ? $"[{ message.IssuingMessageType }] " : string.Empty) }{ error_message }");
                    return Task.CompletedTask;
                },
                async (peer, message, _) =>
                {
                    if (message.ErrorType == EErrorType.Invalid)
                    {
                        await SendErrorMessageToPeerAsync<ErrorMessageData>(peer, EErrorType.InvalidErrorType, "An error message with an invalid error type has been sent.");
                    }
                    else if (string.IsNullOrWhiteSpace(message.MessageType))
                    {
                        await SendErrorMessageToPeerAsync<ErrorMessageData>(peer, EErrorType.InvalidMessageType, "An error message with an invalid message type has been sent.");
                    }
                    else if (message.Message == null)
                    {
                        await SendErrorMessageToPeerAsync<ErrorMessageData>(peer, EErrorType.MessageIsNull, "An error message with message being null has been sent.");
                    }
                    else
                    {
                        await SendUnknownErrorMessageToPeerAsync<ErrorMessageData>(peer, "Message validation has failed.");
                    }
                },
                MessageParseFailedEvent<ErrorMessageData>
            );
            AddMessageParser<TAuthenticationMessageData>
            (
                async (peer, message, _) =>
                {
                    IUser user = await HandlePeerAuthenticationAsync(peer, message);
                    if (user.Peer == peer)
                    {
                        userAuthenticatedEvents.Enqueue(user);
                    }
                },
                FatalPeerMessageValidationFailedEvent<TAuthenticationMessageData>,
                FatalMessageParseFailedEvent<TAuthenticationMessageData>
            );
        }

        /// <summary>
        /// Parses peer message
        /// </summary>
        /// <param name="peer">Peer</param>
        /// <param name="bytes">Bytes</param>
        private void ParsePeerMessage(IPeer peer, ReadOnlySpan<byte> bytes)
        {
            BaseMessageData base_network_message_data = Serializer.Deserialize<BaseMessageData>(bytes);
            if ((base_network_message_data != null) && base_network_message_data.IsValid)
            {
                if (messageParsers.TryGetValue(base_network_message_data.MessageType!, out List<IBaseMessageParser> message_parsers))
                {
                    foreach (IBaseMessageParser message_parser in message_parsers)
                    {
                        message_parser.ParseMessage(peer, bytes);
                    }
                }
                else
                {
                    OnUnknownPeerMessageReceived?.Invoke(peer, base_network_message_data, bytes);
                }
            }
        }

        /// <summary>
        /// Listens to any message parse failed event
        /// </summary>
        /// <param name="peer">Peer</param>
        /// <param name="expectedMessageType">Expected message type</param>
        /// <param name="bytes">Message bytes</param>
        /// <param name="isFatal">Is error fatal</param>
        /// <returns>Task</returns>
        protected Task MessageParseFailedEvent<T>(IPeer peer, string expectedMessageType, ReadOnlyMemory<byte> bytes, bool isFatal) where T : IBaseMessageData => SendErrorMessageToPeerAsync<T>(peer, EErrorType.InvalidMessageParameters, $"Message is invalid. Expected message type: \"{ expectedMessageType }\"{ Environment.NewLine }{ Environment.NewLine }Bytes:{ Environment.NewLine }{ Convert.ToBase64String(bytes.Span) }", isFatal);

        /// <summary>
        /// Listens to any message parse failed event
        /// </summary>
        /// <param name="peer">Peer</param>
        /// <param name="expectedMessageType">Expected message tyoe</param>
        /// <param name="bytes">Bytes</param>
        /// <returns>Task</returns>
        protected Task MessageParseFailedEvent<T>(IPeer peer, string expectedMessageType, ReadOnlyMemory<byte> bytes) where T : IBaseMessageData => MessageParseFailedEvent<T>(peer, expectedMessageType, bytes, false);

        /// <summary>
        /// Listens to any message parse failed event that is fatal
        /// </summary>
        /// <param name="peer">peer</param>
        /// <param name="expectedMessageType">Expected message type</param>
        /// <param name="bytes">Message bytes</param>
        /// <returns>Task</returns>
        protected Task FatalMessageParseFailedEvent<T>(IPeer peer, string expectedMessageType, ReadOnlyMemory<byte> bytes) where T : IBaseMessageData => MessageParseFailedEvent<T>(peer, expectedMessageType, bytes, true);

        /// <summary>
        /// Listens to any message validation event
        /// </summary>
        /// <typeparam name="T">Message type</typeparam>
        /// <param name="peer">Peer</param>
        /// <param name="message">Received message</param>
        /// <param name="bytes">Message bytes</param>
        /// <param name="isFatal">Is validation fail fatal</param>
        /// <returns>Task</returns>
        protected Task MessageValidationFailedEvent<T>(IPeer peer, T message, ReadOnlyMemory<byte> bytes, bool isFatal) where T : IBaseMessageData => SendErrorMessageToPeerAsync<T>(peer, EErrorType.InvalidMessageParameters, $"Message is invalid. Message type: \"{ message.GetType().FullName }\"{ Environment.NewLine }{ Environment.NewLine }Bytes:{ Environment.NewLine }{ Convert.ToBase64String(bytes.Span) }", isFatal);

        /// <summary>
        /// Listens to any message validation event
        /// </summary>
        /// <typeparam name="T">Message type</typeparam>
        /// <param name="peer">Peer</param>
        /// <param name="message">Received message</param>
        /// <param name="bytes">Message bytes</param>
        /// <returns>Task</returns>
        protected Task MessageValidationFailedEvent<T>(IPeer peer, T message, ReadOnlyMemory<byte> bytes) where T : IBaseMessageData => MessageValidationFailedEvent(peer, message, bytes, false);

        /// <summary>
        /// Listens to any message validation event that is fatal
        /// </summary>
        /// <typeparam name="T">Message type</typeparam>
        /// <param name="peer">Peer</param>
        /// <param name="message">Received message</param>
        /// <param name="bytes">Message bytes</param>
        /// <returns>Task</returns>
        protected Task FatalPeerMessageValidationFailedEvent<T>(IPeer peer, T message, ReadOnlyMemory<byte> bytes) where T : IBaseMessageData => MessageValidationFailedEvent(peer, message, bytes, true);

        /// <summary>
        /// Adds an automatic message parser
        /// </summary>
        /// <typeparam name="T">Message type</typeparam>
        /// <param name="onPeerMessageParsed">Gets invoked when a peer message has been parsed</param>
        /// <param name="isFatal">Is validation fail or error fatal</param>
        /// <returns>Message parser</returns>
        protected IPeerMessageParser<T> AddAutomaticMessageParser<T>(PeerMessageParsedDelegate<T> onPeerMessageParsed, bool isFatal) where T : IBaseMessageData => AddMessageParser(onPeerMessageParsed, isFatal ? (PeerMessageValidationFailedDelegate<T>)FatalPeerMessageValidationFailedEvent : MessageValidationFailedEvent, isFatal ? (PeerMessageParseFailedDelegate)FatalMessageParseFailedEvent<T> : MessageParseFailedEvent<T>);

        /// <summary>
        /// Adds an automatic message parser
        /// </summary>
        /// <typeparam name="T">Message type</typeparam>
        /// <param name="onPeerMessageParsed">Gets invoked when a peer message has been parsed</param>
        /// <returns>Message parser</returns>
        protected IPeerMessageParser<T> AddAutomaticMessageParser<T>(PeerMessageParsedDelegate<T> onPeerMessageParsed) where T : IBaseMessageData => AddAutomaticMessageParser(onPeerMessageParsed, false);

        /// <summary>
        /// Adds an automatic message parser that is fatal on validation fail or error
        /// </summary>
        /// <typeparam name="T">Message type</typeparam>
        /// <param name="onPeerMessageParsed">Gets invoked when a peer message has been parsed</param>
        /// <returns>Message parser</returns>
        protected IPeerMessageParser<T> AddAutomaticMessageParserWithFatality<T>(PeerMessageParsedDelegate<T> onPeerMessageParsed) where T : IBaseMessageData => AddAutomaticMessageParser(onPeerMessageParsed, false);

        /// <summary>
        /// Handles peer authentication asynchronously
        /// </summary>
        /// <param name="peer">Peer</param>
        /// <param name="authenticationMessageData">Authentication message data</param>
        /// <returns>"true" if authentication was successful, otherwise "false"</returns>
        protected abstract Task<IUser> HandlePeerAuthenticationAsync(IPeer peer, TAuthenticationMessageData authenticationMessageData);

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
                connector.OnPeerConnectionAttempted += (peer) => OnPeerConnectionAttempted?.Invoke(peer);
                connector.OnPeerConnected += (peer) => OnPeerConnected?.Invoke(peer);
                connector.OnPeerDisconnected += (peer, disconnectionReason) =>
                {
                    if (TryGettingUserFromPeer(peer, out IUser user))
                    {
                        OnUserDisconnected?.Invoke(user);
                        users.Remove(user.GUID);
                    }
                    peerGUIDToUserLookup.Remove(peer.GUID);
                    OnPeerDisconnected?.Invoke(peer, disconnectionReason);
                };
                connector.OnPeerMessageReceived += (peer, message) =>
                {
                    OnPeerMessageReceived?.Invoke(peer, message);
                    ParsePeerMessage(peer, message);
                };
            }
            return ret;
        }

        /// <summary>
        /// Removes the specified connector
        /// </summary>
        /// <param name="connector">Connector</param>
        /// <returns>"true" if the specified connector was successfully removed, otherwise "false"</returns>
        public bool RemoveConnector(IConnector connector) => connectors.Remove(connector);

        /// <summary>
        /// Gets a connector with the specified type
        /// </summary>
        /// <typeparam name="T">Connector type</typeparam>
        /// <returns>Connector of specified type if successful, otherwise "null"</returns>
        public T GetConnectorOfType<T>() where T : IConnector?
        {
            TryGetConnectorOfType(out T ret);
            return ret;
        }

        /// <summary>
        /// Tries to get a connector of the specified type
        /// </summary>
        /// <typeparam name="T">Connector type</typeparam>
        /// <param name="connector">Connector</param>
        /// <returns>"true" if connector of the specified type is available, otherwise "false"</returns>
        public bool TryGetConnectorOfType<T>(out T connector) where T : IConnector?
        {
            bool ret = false;
            connector = default!;
            foreach (IConnector available_connector in connectors)
            {
                if (available_connector is T result)
                {
                    connector = result;
                    ret = true;
                    break;
                }
            }
            return ret;
        }

        /// <summary>
        /// Sends a message to peer asynchronously
        /// </summary>
        /// <typeparam name="T">Message type</typeparam>
        /// <param name="peer">Peer</param>
        /// <param name="message">Message</param>
        /// <returns>Task</returns>
        public Task SendMessageToPeerAsync<T>(IPeer peer, T message) where T : IBaseMessageData
        {
            if (peer == null)
            {
                throw new ArgumentNullException(nameof(peer));
            }
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }
            return peer.SendMessageAsync(Serializer.Serialize(message).ToArray());
        }

        /// <summary>
        /// Adds a message parser
        /// </summary>
        /// <typeparam name="T">Message type</typeparam>
        /// <param name="onPeerMessageParsed">Gets invoked when a peer message has been parsed</param>
        /// <param name="onPeerMessageValidationFailed">Gets invoked when validating a peer message has failed</param>
        /// <param name="onPeerMessageParseFailed">Gets invoked when parsing a peer message has failed</param>
        /// <returns>Message parser</returns>
        public IPeerMessageParser<T> AddMessageParser<T>(PeerMessageParsedDelegate<T> onPeerMessageParsed, PeerMessageValidationFailedDelegate<T>? onPeerMessageValidationFailed = null, PeerMessageParseFailedDelegate? onPeerMessageParseFailed = null) where T : IBaseMessageData
        {
            IPeerMessageParser<T> ret = new PeerMessageParser<T>(Serializer, onPeerMessageParsed, onPeerMessageValidationFailed, onPeerMessageParseFailed);
            if (!messageParsers.TryGetValue(ret.MessageType, out List<IBaseMessageParser> message_parsers))
            {
                message_parsers = new List<IBaseMessageParser>();
                messageParsers.Add(ret.MessageType, message_parsers);
            }
            message_parsers.Add(ret);
            return ret;
        }

        /// <summary>
        /// Gets message parsers for the specified type
        /// </summary>
        /// <typeparam name="T">Message type</typeparam>
        /// <returns>Message parsers if successful, otherwise "null"</returns>
        public IEnumerable<IPeerMessageParser<T>> GetMessageParsersForType<T>() where T : IBaseMessageData => TryGetMessageParsersForType(out IEnumerable<IPeerMessageParser<T>>? ret) ? ret! : Array.Empty<IPeerMessageParser<T>>();

        /// <summary>
        /// Tries to get message parsers for the specified type
        /// </summary>
        /// <typeparam name="T">Message type</typeparam>
        /// <param name="messageParsers">Message parsers</param>
        /// <returns>"true" if message parsers are available, otherwise "false"</returns>
        public bool TryGetMessageParsersForType<T>(out IEnumerable<IPeerMessageParser<T>>? messageParsers) where T : IBaseMessageData
        {
            string key = Naming.GetMessageTypeNameFromMessageDataType<T>();
            bool ret = this.messageParsers.TryGetValue(key, out List<IBaseMessageParser> message_parsers);
            if (ret)
            {
                List<IPeerMessageParser<T>> message_parser_list = new List<IPeerMessageParser<T>>();
                foreach (IBaseMessageParser base_message_parser in message_parsers)
                {
                    if (base_message_parser is IPeerMessageParser<T> message_parser)
                    {
                        message_parser_list.Add(message_parser);
                    }
                }
                messageParsers = message_parser_list;
            }
            else
            {
                messageParsers = null;
            }
            return ret;
        }

        /// <summary>
        /// Removes the specified message parser
        /// </summary>
        /// <typeparam name="T">Message type</typeparam>
        /// <param name="messageParser">Message parser</param>
        /// <returns>"true" if message parser was successfully removed, otherwise "false"</returns>
        public bool RemoveMessageParser<T>(IPeerMessageParser<T> messageParser) where T : IBaseMessageData
        {
            if (messageParser == null)
            {
                throw new ArgumentNullException(nameof(messageParser));
            }
            bool ret = false;
            if (messageParsers.TryGetValue(messageParser.MessageType, out List<IBaseMessageParser> message_parsers))
            {
                ret = message_parsers.Remove(messageParser);
                if (ret && (message_parsers.Count <= 0))
                {
                    messageParsers.Remove(messageParser.MessageType);
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
        public bool TryGettingUserFromPeer(IPeer peer, out IUser user) =>
            peerGUIDToUserLookup.TryGetValue(peer.GUID, out user) && (user.Peer == peer);

        /// <summary>
        /// Asserts that the specified peer is an user
        /// </summary>
        /// <param name="peer">Peer</param>
        /// <param name="onPeerIsAnUserAsserted">Gets invoked when the specified peer is an user</param>
        public void AssertPeerIsAnUser(IPeer peer, PeerIsAnUserAssertedDelegate onPeerIsAnUserAsserted)
        {
            if (TryGettingUserFromPeer(peer, out IUser user))
            {
                onPeerIsAnUserAsserted(user);
            }
        }

        /// <summary>
        /// Sends an error message to peer asynchronously
        /// </summary>
        /// <typeparam name="T">Message type</typeparam>
        /// <param name="peer">Peer</param>
        /// <param name="errorType">Error type</param>
        /// <param name="errorMessage">Error message</param>
        public Task SendErrorMessageToPeerAsync<T>(IPeer peer, EErrorType errorType, string errorMessage) where T : IBaseMessageData => SendErrorMessageToPeerAsync<T>(peer, errorType, errorMessage, false);

        /// <summary>
        /// Sends an error message to peer asynchronously
        /// </summary>
        /// <typeparam name="T">Message type</typeparam>
        /// <param name="peer">Peer</param>
        /// <param name="errorType">Error tyoe</param>
        /// <param name="errorMessage">Error message</param>
        /// <param name="isFatal">Is error fatal</param>
        /// <returns>Task</returns>
        public async Task SendErrorMessageToPeerAsync<T>(IPeer peer, EErrorType errorType, string errorMessage, bool isFatal) where T : IBaseMessageData
        {
            Console.Error.WriteLine($"[{ errorType }] { errorMessage }");
            await SendMessageToPeerAsync(peer, new ErrorMessageData(errorType, Naming.GetMessageTypeNameFromMessageDataType<T>(), errorMessage));
            if (isFatal)
            {
                peer.Disconnect(EDisconnectionReason.Error);
            }
        }

        /// <summary>
        /// Sends an invalid message parameters error message to peer asynchronously
        /// </summary>
        /// <typeparam name="T">Message type</typeparam>
        /// <param name="peer">Peer</param>
        /// <param name="errorMessage">Error message</param>
        public Task SendInvalidMessageParametersErrorMessageToPeerAsync<T>(IPeer peer, string errorMessage) where T : IBaseMessageData => SendErrorMessageToPeerAsync<T>(peer, EErrorType.InvalidMessageParameters, errorMessage);

        /// <summary>
        /// Sends an invalid message context error message to peer asynchronously
        /// </summary>
        /// <typeparam name="T">Message type</typeparam>
        /// <param name="peer">Peer</param>
        /// <param name="errorMessage">Error message</param>
        /// <returns>Task</returns>
        public Task SendInvalidMessageContextErrorMessageToPeerAsync<T>(IPeer peer, string errorMessage) where T : IBaseMessageData => SendErrorMessageToPeerAsync<T>(peer, EErrorType.InvalidMessageContext, errorMessage);

        /// <summary>
        /// Sends an unknown error message to peer asynchronously
        /// </summary>
        /// <typeparam name="T">Message type</typeparam>
        /// <param name="peer">Peer</param>
        /// <param name="errorMessage">Error message</param>
        /// <returns>Task</returns>
        public Task SendUnknownErrorMessageToPeerAsync<T>(IPeer peer, string errorMessage) where T : IBaseMessageData => SendErrorMessageToPeerAsync<T>(peer, EErrorType.Unknown, errorMessage);

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
            while (userAuthenticatedEvents.TryDequeue(out IUser user))
            {
                if (users.TryAdd(user.GUID, user) && peerGUIDToUserLookup.TryAdd(user.Peer.GUID, user))
                {
                    OnUserAuthenticated?.Invoke(user);
                }
            }
        }

        /// <summary>
        /// Disposes this object
        /// </summary>
        public void Dispose() => Close(EDisconnectionReason.Disposed);
    }
}
