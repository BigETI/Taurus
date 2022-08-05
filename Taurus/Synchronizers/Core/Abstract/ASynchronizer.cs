using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using System;
using System.Collections.Generic;
using System.IO;
using Taurus.Connectors;
using Taurus.Serializers;
using Taurus.Synchronizers.Data.Messages;

namespace Taurus.Synchronizers
{
    public abstract class ASynchronizer : ISynchronizer
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
        /// JSON serializer
        /// </summary>
        private readonly JsonSerializer jsonSerializer = new JsonSerializer();

        /// <summary>
        /// Available connectors
        /// </summary>
        public IEnumerable<IConnector> Connectors => connectors;

        /// <summary>
        /// Serializer
        /// </summary>
        public ISerializer Serializer { get; }

        /// <summary>
        /// This event will be invoked when a peer has attempted to connect to any of the available connectors.
        /// </summary>
        public event PeerConnectionAttemptedDelegate? OnPeerConnectionAttempted;

        /// <summary>
        /// This event will be invoked when a peer has successfully connected to any of the available connectors.
        /// </summary>
        public event PeerConnectedDelegate? OnPeerConnected;

        /// <summary>
        /// This event will be invoked when a peer has been disconnected from any of the available connectors.
        /// </summary>
        public event PeerDisconnectedDelegate? OnPeerDisconnected;

        /// <summary>
        /// This event will be invoked when a message has been received from a peer.
        /// </summary>
        public event PeerMessageReceivedDelegate? OnPeerMessageReceived;

        /// <summary>
        /// This event will be invoked when a non-meaningful message has been received from a peer.
        /// </summary>
        public event UnknownMessageReceivedDelegate? OnUnknownMessageReceived;

        /// <summary>
        /// This event will be invoked when an error has been received.
        /// </summary>
        public event ErrorMessageReceivedDelegate? OnErrorMessageReceived;

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
                    OnErrorMessageReceived?.Invoke(message.ErrorType, message.Message ?? string.Empty);
                    Console.Error.WriteLine($"[{ message.ErrorType }] { message.Message ?? string.Empty }");
                },
                (peer, message, _) =>
                {
                    if (message.ErrorType == EErrorType.Invalid)
                    {
                        SendErrorMessageToPeer<ErrorMessageData>(peer, EErrorType.InvalidErrorType, "An error message with an invalid error type has been sent.");
                    }
                    else if (string.IsNullOrWhiteSpace(message.MessageType))
                    {
                        SendErrorMessageToPeer<ErrorMessageData>(peer, EErrorType.InvalidMessageType, "An error message with an invalid message type has been sent.");
                    }
                    else if (message.Message == null)
                    {
                        SendErrorMessageToPeer<ErrorMessageData>(peer, EErrorType.MessageIsNull, "An error message with message being null has been sent.");
                    }
                    else
                    {
                        SendUnknownErrorMessageToPeer<ErrorMessageData>(peer, "Message validation has failed.");
                    }
                },
                MessageParseFailedEvent<ErrorMessageData>
            );
        }

        /// <summary>
        /// Listens to any message parse failed event
        /// </summary>
        /// <param name="peer">Peer</param>
        /// <param name="expectedMessageType">Expected message type</param>
        /// <param name="bson">BSON</param>
        /// <param name="isFatal">Is error fatal</param>
        protected void MessageParseFailedEvent<T>(IPeer peer, string expectedMessageType, ReadOnlySpan<byte> bson, bool isFatal) where T : IBaseMessageData => SendErrorMessageToPeer<T>(peer, EErrorType.InvalidMessageParameters, $"Message is invalid. Expected message type: \"{ expectedMessageType }\"{ Environment.NewLine }{ Environment.NewLine }BSON:{ Environment.NewLine }{ Convert.ToBase64String(bson) }", isFatal);

        /// <summary>
        /// Listens to any message parse failed event
        /// </summary>
        /// <param name="peer">Peer</param>
        /// <param name="expectedMessageType">Expected message tyoe</param>
        /// <param name="bson">BSON</param>
        protected void MessageParseFailedEvent<T>(IPeer peer, string expectedMessageType, ReadOnlySpan<byte> bson) where T : IBaseMessageData => MessageParseFailedEvent<T>(peer, expectedMessageType, bson, false);

        /// <summary>
        /// Listens to any message parse failed event that is fatal
        /// </summary>
        /// <param name="peer">peer</param>
        /// <param name="expectedMessageType">Expected message type</param>
        /// <param name="bson">BSON</param>
        protected void FatalMessageParseFailedEvent<T>(IPeer peer, string expectedMessageType, ReadOnlySpan<byte> bson) where T : IBaseMessageData => MessageParseFailedEvent<T>(peer, expectedMessageType, bson, true);

        /// <summary>
        /// Listens to any message validation event
        /// </summary>
        /// <typeparam name="T">Message type</typeparam>
        /// <param name="peer">Peer</param>
        /// <param name="message">Received message</param>
        /// <param name="bson">Message BSON</param>
        /// <param name="isFatal">Is validation fail fatal</param>
        protected void MessageValidationFailedEvent<T>(IPeer peer, T message, ReadOnlySpan<byte> bson, bool isFatal) where T : IBaseMessageData => SendErrorMessageToPeer<T>(peer, EErrorType.InvalidMessageParameters, $"Message is invalid. Message type: \"{ message.GetType().FullName }\"{ Environment.NewLine }{ Environment.NewLine }JSON:{ Environment.NewLine }{ Convert.ToBase64String(bson) }", isFatal);

        /// <summary>
        /// Listens to any message validation event
        /// </summary>
        /// <typeparam name="T">Message type</typeparam>
        /// <param name="peer">Peer</param>
        /// <param name="message">Received message</param>
        /// <param name="bson">Message BSON</param>
        protected void MessageValidationFailedEvent<T>(IPeer peer, T message, ReadOnlySpan<byte> bson) where T : IBaseMessageData => MessageValidationFailedEvent(peer, message, bson, false);

        /// <summary>
        /// Listens to any message validation event that is fatal
        /// </summary>
        /// <typeparam name="T">Message type</typeparam>
        /// <param name="peer">Peer</param>
        /// <param name="message">Received message</param>
        /// <param name="bson">Message BSON</param>
        protected void FatalMessageValidationFailedEvent<T>(IPeer peer, T message, ReadOnlySpan<byte> bson) where T : IBaseMessageData => MessageValidationFailedEvent(peer, message, bson, true);

        /// <summary>
        /// Adds an automatic message parser
        /// </summary>
        /// <typeparam name="T">Message type</typeparam>
        /// <param name="onMessageParsed">On message parsed</param>
        /// <param name="isFatal">Is validation fail or error fatal</param>
        /// <returns>Message parser</returns>
        protected IMessageParser<T> AddAutomaticMessageParser<T>(MessageParsedDelegate<T> onMessageParsed, bool isFatal) where T : IBaseMessageData => AddMessageParser(onMessageParsed, isFatal ? (MessageValidationFailedDelegate<T>)FatalMessageValidationFailedEvent : MessageValidationFailedEvent, isFatal ? (MessageParseFailedDelegate)FatalMessageParseFailedEvent<T> : MessageParseFailedEvent<T>);

        /// <summary>
        /// Adds an automatic message parser
        /// </summary>
        /// <typeparam name="T">Message type</typeparam>
        /// <param name="onMessageParsed">On message parsed</param>
        /// <returns>Message parser</returns>
        protected IMessageParser<T> AddAutomaticMessageParser<T>(MessageParsedDelegate<T> onMessageParsed) where T : IBaseMessageData => AddAutomaticMessageParser(onMessageParsed, false);

        /// <summary>
        /// Adds an automatic message parser that is fatal on validation fail or error
        /// </summary>
        /// <typeparam name="T">Message type</typeparam>
        /// <param name="onMessageParsed">On message parsed</param>
        /// <returns>Message parser</returns>
        protected IMessageParser<T> AddAutomaticMessageParserWithFatality<T>(MessageParsedDelegate<T> onMessageParsed) where T : IBaseMessageData => AddAutomaticMessageParser(onMessageParsed, false);

        /// <summary>
        /// Add connector
        /// </summary>
        /// <param name="connector">Connector</param>
        /// <returns>"true" if connector was successfully added, otherwise "false"</returns>
        public bool AddConnector(IConnector connector)
        {
            bool ret = !connectors.Contains(connector);
            if (ret)
            {
                connectors.Add(connector);
                connector.OnPeerConnectionAttempted += (peer) => OnPeerConnectionAttempted?.Invoke(peer);
                connector.OnPeerConnected += (peer) => OnPeerConnected?.Invoke(peer);
                connector.OnPeerDisconnected += (peer, disconnectionReason) => OnPeerDisconnected?.Invoke(peer, disconnectionReason);
                connector.OnPeerMessageReceived += (peer, message) =>
                {
                    OnPeerMessageReceived?.Invoke(peer, message);
                    ParseMessage(peer, message);
                };
            }
            return ret;
        }

        /// <summary>
        /// Remove connector
        /// </summary>
        /// <param name="connector">Connector</param>
        /// <returns>"true" if connector was successfully removed, otherwise "false"</returns>
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
        /// Sends a message to peer
        /// </summary>
        /// <typeparam name="T">Message type</typeparam>
        /// <param name="peer">Peer</param>
        /// <param name="message">Message</param>
        public void SendMessageToPeer<T>(IPeer peer, T message) where T : IBaseMessageData
        {
            if (peer == null)
            {
                throw new ArgumentNullException(nameof(peer));
            }
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }
            using MemoryStream memory_stream = new MemoryStream();
            using (BsonDataWriter bson_data_writer = new BsonDataWriter(memory_stream))
            {
                jsonSerializer.Serialize(bson_data_writer, message);
            }
            memory_stream.Seek(0L, SeekOrigin.Begin);
            peer.SendMessage(memory_stream.ToArray());
        }

        /// <summary>
        /// Adds a message parser
        /// </summary>
        /// <typeparam name="T">Message type</typeparam>
        /// <param name="onMessageParsed">On message parsed</param>
        /// <param name="onMessageValidationFailed">On message validation failed</param>
        /// <param name="onMessageParseFailed">On message parse failed</param>
        /// <returns>Message parser</returns>
        public IMessageParser<T> AddMessageParser<T>(MessageParsedDelegate<T> onMessageParsed, MessageValidationFailedDelegate<T>? onMessageValidationFailed = null, MessageParseFailedDelegate? onMessageParseFailed = null) where T : IBaseMessageData
        {
            IMessageParser<T> ret = new MessageParser<T>(Serializer, onMessageParsed, onMessageValidationFailed, onMessageParseFailed);
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
        public IEnumerable<IMessageParser<T>> GetMessageParsersForType<T>() where T : IBaseMessageData => TryGetMessageParsersForType(out IEnumerable<IMessageParser<T>>? ret) ? ret! : Array.Empty<IMessageParser<T>>();

        /// <summary>
        /// Tries to get message parsers for the specified type
        /// </summary>
        /// <typeparam name="T">Message type</typeparam>
        /// <param name="messageParsers">Message parsers</param>
        /// <returns>"true" if message parsers are available, otherwise "false"</returns>
        public bool TryGetMessageParsersForType<T>(out IEnumerable<IMessageParser<T>>? messageParsers) where T : IBaseMessageData
        {
            string key = Naming.GetMessageTypeNameFromMessageDataType<T>();
            bool ret = this.messageParsers.TryGetValue(key, out List<IBaseMessageParser> message_parsers);
            if (ret)
            {
                List<IMessageParser<T>> message_parser_list = new List<IMessageParser<T>>();
                foreach (IBaseMessageParser base_message_parser in message_parsers)
                {
                    if (base_message_parser is IMessageParser<T> message_parser)
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
        public bool RemoveMessageParser<T>(IMessageParser<T> messageParser) where T : IBaseMessageData
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
        /// Parses incoming message
        /// </summary>
        /// <param name="peer">Peer</param>
        /// <param name="bytes">BYtes</param>
        public void ParseMessage(IPeer peer, ReadOnlySpan<byte> bytes)
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
                    OnUnknownMessageReceived?.Invoke(base_network_message_data, bytes);
                }
            }
        }

        /// <summary>
        /// Sends an error message to peer
        /// </summary>
        /// <typeparam name="T">Message type</typeparam>
        /// <param name="peer">Peer</param>
        /// <param name="errorType">Error type</param>
        /// <param name="errorMessage">Error message</param>
        public void SendErrorMessageToPeer<T>(IPeer peer, EErrorType errorType, string errorMessage) where T : IBaseMessageData => SendErrorMessageToPeer<T>(peer, errorType, errorMessage, false);

        /// <summary>
        /// Sends an error message to peer
        /// </summary>
        /// <typeparam name="T">Message type</typeparam>
        /// <param name="peer">Peer</param>
        /// <param name="errorType">Error tyoe</param>
        /// <param name="errorMessage">Error message</param>
        /// <param name="isFatal">Is error fatal</param>
        public void SendErrorMessageToPeer<T>(IPeer peer, EErrorType errorType, string errorMessage, bool isFatal) where T : IBaseMessageData
        {
            Console.Error.WriteLine($"[{ errorType }] { errorMessage }");
            SendMessageToPeer(peer, new ErrorMessageData(errorType, Naming.GetMessageTypeNameFromMessageDataType<T>(), errorMessage));
            if (isFatal)
            {
                peer.Disconnect(EDisconnectionReason.Error);
            }
        }

        /// <summary>
        /// Sends an invalid message parameters error message to peer
        /// </summary>
        /// <typeparam name="T">Message type</typeparam>
        /// <param name="peer">Peer</param>
        /// <param name="errorMessage">Error message</param>
        public void SendInvalidMessageParametersErrorMessageToPeer<T>(IPeer peer, string errorMessage) where T : IBaseMessageData => SendErrorMessageToPeer<T>(peer, EErrorType.InvalidMessageParameters, errorMessage);

        /// <summary>
        /// Sends an invalid message context error message to peer
        /// </summary>
        /// <typeparam name="T">Message type</typeparam>
        /// <param name="peer">Peer</param>
        /// <param name="errorMessage">Error message</param>
        public void SendInvalidMessageContextErrorMessageToPeer<T>(IPeer peer, string errorMessage) where T : IBaseMessageData => SendErrorMessageToPeer<T>(peer, EErrorType.InvalidMessageContext, errorMessage);

        /// <summary>
        /// Sends an unknown error message to peer
        /// </summary>
        /// <typeparam name="T">Message type</typeparam>
        /// <param name="peer">Peer</param>
        /// <param name="errorMessage">Error message</param>
        public void SendUnknownErrorMessageToPeer<T>(IPeer peer, string errorMessage) where T : IBaseMessageData => SendErrorMessageToPeer<T>(peer, EErrorType.Unknown, errorMessage);

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
        /// Dispose object
        /// </summary>
        public void Dispose() => Close(EDisconnectionReason.Disposed);
    }
}
