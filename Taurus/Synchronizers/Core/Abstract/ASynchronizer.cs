using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Taurus.Connectors;
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
    public abstract class ASynchronizer : ISynchronizer
    {
        /// <summary>
        /// Available connectors
        /// </summary>
        private readonly List<IConnector> connectors = new List<IConnector>();

        /// <summary>
        /// Available message parsers
        /// </summary>
        private readonly Dictionary<string, List<IBasePeerMessageParser>> messageParsers = new Dictionary<string, List<IBasePeerMessageParser>>();

        /// <summary>
        /// Available connectors
        /// </summary>
        public IEnumerable<IConnector> Connectors => connectors;

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
        /// Constructs a generalised synchronizer object
        /// </summary>
        /// <param name="serializer">Serializer</param>
        public ASynchronizer(ISerializer serializer)
        {
            Serializer = serializer;
            AddPeerMessageParser<ErrorMessageData>
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
                        await SendErrorMessageToPeerAsync<ErrorMessageData>
                        (
                            peer,
                            EErrorType.InvalidErrorType,
                            "An error message with an invalid error type has been sent."
                        );
                    }
                    else if (string.IsNullOrWhiteSpace(message.MessageType))
                    {
                        await SendErrorMessageToPeerAsync<ErrorMessageData>
                        (
                            peer,
                            EErrorType.InvalidMessageType,
                            "An error message with an invalid message type has been sent."
                        );
                    }
                    else if (message.Message == null)
                    {
                        await SendErrorMessageToPeerAsync<ErrorMessageData>
                        (
                            peer,
                            EErrorType.MessageIsNull,
                            "An error message with message being null has been sent."
                        );
                    }
                    else
                    {
                        await SendUnknownErrorMessageToPeerAsync<ErrorMessageData>(peer, "Message validation has failed.");
                    }
                },
                PeerMessageParseFailedEvent<ErrorMessageData>
            );
        }

        /// <summary>
        /// Parses peer message
        /// </summary>
        /// <param name="peer">Peer</param>
        /// <param name="bytes">Bytes</param>
        private void ParsePeerMessage(IPeer peer, ReadOnlySpan<byte> bytes)
        {
            BaseMessageData? base_network_message_data = Serializer.Deserialize<BaseMessageData?>(bytes);
            if (Validator.IsValid(base_network_message_data))
            {
                if (messageParsers.TryGetValue(base_network_message_data!.MessageType!, out List<IBasePeerMessageParser> message_parsers))
                {
                    foreach (IBasePeerMessageParser message_parser in message_parsers)
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
        /// Listens to any peer message parse failed event
        /// </summary>
        /// <typeparam name="TMessageData">Message data type</typeparam>
        /// <param name="peer">Peer</param>
        /// <param name="expectedMessageType">Expected message type</param>
        /// <param name="bytes">Message bytes</param>
        /// <param name="isFatal">Is error fatal</param>
        /// <returns>Task</returns>
        protected Task PeerMessageParseFailedEvent<TMessageData>(IPeer peer, string expectedMessageType, ReadOnlyMemory<byte> bytes, bool isFatal)
            where TMessageData : IBaseMessageData =>
            SendErrorMessageToPeerAsync<TMessageData>
            (
                peer,
                EErrorType.InvalidMessageParameters,
                $"Message is invalid. Expected message type: \"{ expectedMessageType }\"{ Environment.NewLine }{ Environment.NewLine }Bytes:{ Environment.NewLine }{ Convert.ToBase64String(bytes.Span) }",
                isFatal
            );

        /// <summary>
        /// Listens to any peer message parse failed event
        /// </summary>
        /// <typeparam name="TMessageData">Message data type</typeparam>
        /// <param name="peer">Peer</param>
        /// <param name="expectedMessageType">Expected message tyoe</param>
        /// <param name="bytes">Bytes</param>
        /// <returns>Task</returns>
        protected Task PeerMessageParseFailedEvent<TMessageData>(IPeer peer, string expectedMessageType, ReadOnlyMemory<byte> bytes)
            where TMessageData : IBaseMessageData =>
            PeerMessageParseFailedEvent<TMessageData>(peer, expectedMessageType, bytes, false);

        /// <summary>
        /// Listens to any peer message parse failed event that is fatal
        /// </summary>
        /// <typeparam name="TMessageData">Message data type</typeparam>
        /// <param name="peer">Peer</param>
        /// <param name="expectedMessageType">Expected message type</param>
        /// <param name="bytes">Message bytes</param>
        /// <returns>Task</returns>
        protected Task FatalPeerMessageParseFailedEvent<TMessageData>(IPeer peer, string expectedMessageType, ReadOnlyMemory<byte> bytes)
            where TMessageData : IBaseMessageData =>
            PeerMessageParseFailedEvent<TMessageData>(peer, expectedMessageType, bytes, true);

        /// <summary>
        /// Listens to any peer message validation event
        /// </summary>
        /// <typeparam name="TMessageData">Message data type</typeparam>
        /// <param name="peer">Peer</param>
        /// <param name="message">Received message</param>
        /// <param name="bytes">Message bytes</param>
        /// <param name="isFatal">Is validation fail fatal</param>
        /// <returns>Task</returns>
        protected Task PeerMessageValidationFailedEvent<TMessageData>(IPeer peer, TMessageData message, ReadOnlyMemory<byte> bytes, bool isFatal)
            where TMessageData : IBaseMessageData =>
            SendErrorMessageToPeerAsync<TMessageData>
            (
                peer,
                EErrorType.InvalidMessageParameters,
                $"Message is invalid. Message type: \"{ message.GetType().FullName }\"{ Environment.NewLine }{ Environment.NewLine }Bytes:{ Environment.NewLine }{ Convert.ToBase64String(bytes.Span) }",
                isFatal
            );

        /// <summary>
        /// Listens to any peer message validation event
        /// </summary>
        /// <typeparam name="TMessageData">Message data type</typeparam>
        /// <param name="peer">Peer</param>
        /// <param name="message">Received message</param>
        /// <param name="bytes">Message bytes</param>
        /// <returns>Task</returns>
        protected Task PeerMessageValidationFailedEvent<TMessageData>(IPeer peer, TMessageData message, ReadOnlyMemory<byte> bytes)
            where TMessageData : IBaseMessageData =>
            PeerMessageValidationFailedEvent(peer, message, bytes, false);

        /// <summary>
        /// Listens to any peer message validation event that is fatal
        /// </summary>
        /// <typeparam name="TMessageData">Message data type</typeparam>
        /// <param name="peer">Peer</param>
        /// <param name="message">Received message</param>
        /// <param name="bytes">Message bytes</param>
        /// <returns>Task</returns>
        protected Task FatalPeerMessageValidationFailedEvent<TMessageData>(IPeer peer, TMessageData message, ReadOnlyMemory<byte> bytes)
            where TMessageData : IBaseMessageData =>
            PeerMessageValidationFailedEvent(peer, message, bytes, true);

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
                connector.OnPeerDisconnected += (peer, disconnectionReason) => OnPeerDisconnected?.Invoke(peer, disconnectionReason);
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
        /// <typeparam name="TConnector">Connector type</typeparam>
        /// <returns>Connector of specified type if successful, otherwise "null"</returns>
        public TConnector GetConnectorOfType<TConnector>() where TConnector : IConnector?
        {
            TryGetConnectorOfType(out TConnector ret);
            return ret;
        }

        /// <summary>
        /// Tries to get a connector of the specified type
        /// </summary>
        /// <typeparam name="TConnector">Connector type</typeparam>
        /// <param name="connector">Connector</param>
        /// <returns>"true" if connector of the specified type is available, otherwise "false"</returns>
        public bool TryGetConnectorOfType<TConnector>(out TConnector connector) where TConnector : IConnector?
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
        /// Sends a message to peer asynchronously
        /// </summary>
        /// <typeparam name="TMessageData">Message data type</typeparam>
        /// <param name="peer">Peer</param>
        /// <param name="message">Message</param>
        /// <returns>Task</returns>
        public Task SendMessageToPeerAsync<TMessageData>(IPeer peer, TMessageData message) where TMessageData : IBaseMessageData
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
        /// Adds a peer message parser
        /// </summary>
        /// <typeparam name="TMessageData">Message data type</typeparam>
        /// <param name="onPeerMessageParsed">Gets invoked when a peer message has been parsed</param>
        /// <param name="onPeerMessageValidationFailed">Gets invoked when validating a peer message has failed</param>
        /// <param name="onPeerMessageParseFailed">Gets invoked when parsing a peer message has failed</param>
        /// <returns>Message parser</returns>
        public IPeerMessageParser<TMessageData> AddPeerMessageParser<TMessageData>
        (
            PeerMessageParsedDelegate<TMessageData> onPeerMessageParsed,
            PeerMessageValidationFailedDelegate<TMessageData> onPeerMessageValidationFailed,
            PeerMessageParseFailedDelegate onPeerMessageParseFailed
        ) where TMessageData : IBaseMessageData
        {
            IPeerMessageParser<TMessageData> ret = new PeerMessageParser<TMessageData>(Serializer, onPeerMessageParsed, onPeerMessageValidationFailed, onPeerMessageParseFailed);
            if (!messageParsers.TryGetValue(ret.MessageType, out List<IBasePeerMessageParser> message_parsers))
            {
                message_parsers = new List<IBasePeerMessageParser>();
                messageParsers.Add(ret.MessageType, message_parsers);
            }
            message_parsers.Add(ret);
            return ret;
        }

        /// <summary>
        /// Adds an automatic peer message parser
        /// </summary>
        /// <typeparam name="TMessageData">Message data type</typeparam>
        /// <param name="onPeerMessageParsed">Gets invoked when a peer message has been parsed</param>
        /// <param name="isFatal">Is validation fail or error fatal</param>
        /// <returns>Message parser</returns>
        public IPeerMessageParser<TMessageData> AddAutomaticPeerMessageParser<TMessageData>
        (
            PeerMessageParsedDelegate<TMessageData> onPeerMessageParsed,
            bool isFatal
        ) where TMessageData : IBaseMessageData =>
            AddPeerMessageParser
            (
                onPeerMessageParsed,
                isFatal ? (PeerMessageValidationFailedDelegate<TMessageData>)FatalPeerMessageValidationFailedEvent : PeerMessageValidationFailedEvent,
                isFatal ? (PeerMessageParseFailedDelegate)FatalPeerMessageParseFailedEvent<TMessageData> : PeerMessageParseFailedEvent<TMessageData>
            );

        /// <summary>
        /// Adds an automatic peer message parser
        /// </summary>
        /// <typeparam name="TMessageData">Message data type</typeparam>
        /// <param name="onPeerMessageParsed">Gets invoked when a peer message has been parsed</param>
        /// <returns>Message parser</returns>
        public IPeerMessageParser<TMessageData> AddAutomaticPeerMessageParser<TMessageData>(PeerMessageParsedDelegate<TMessageData> onPeerMessageParsed)
            where TMessageData : IBaseMessageData =>
            AddAutomaticPeerMessageParser(onPeerMessageParsed, false);

        /// <summary>
        /// Adds an automatic peer message parser that is fatal on validation fail or error
        /// </summary>
        /// <typeparam name="TMessageData">Message data type</typeparam>
        /// <param name="onPeerMessageParsed">Gets invoked when a peer message has been parsed</param>
        /// <returns>Message parser</returns>
        public IPeerMessageParser<TMessageData> AddAutomaticPeerMessageParserWithFatality<TMessageData>
        (
            PeerMessageParsedDelegate<TMessageData> onPeerMessageParsed
        ) where TMessageData : IBaseMessageData =>
            AddAutomaticPeerMessageParser(onPeerMessageParsed, true);

        /// <summary>
        /// Gets message parsers for the specified type
        /// </summary>
        /// <typeparam name="TMessageData">Message data type</typeparam>
        /// <returns>Message parsers if successful, otherwise "null"</returns>
        public IEnumerable<IPeerMessageParser<TMessageData>> GetPeerMessageParsersForType<TMessageData>() where TMessageData : IBaseMessageData =>
            TryGetMessageParsersForType(out IEnumerable<IPeerMessageParser<TMessageData>>? ret) ? ret! : Array.Empty<IPeerMessageParser<TMessageData>>();

        /// <summary>
        /// Tries to get message parsers for the specified type
        /// </summary>
        /// <typeparam name="TMessageData">Message data type</typeparam>
        /// <param name="messageParsers">Message parsers</param>
        /// <returns>"true" if message parsers are available, otherwise "false"</returns>
        public bool TryGetMessageParsersForType<TMessageData>(out IEnumerable<IPeerMessageParser<TMessageData>>? messageParsers)
            where TMessageData : IBaseMessageData
        {
            string key = Naming.GetMessageTypeNameFromMessageDataType<TMessageData>();
            bool ret = this.messageParsers.TryGetValue(key, out List<IBasePeerMessageParser> message_parsers);
            if (ret)
            {
                List<IPeerMessageParser<TMessageData>> message_parser_list = new List<IPeerMessageParser<TMessageData>>();
                foreach (IBasePeerMessageParser base_message_parser in message_parsers)
                {
                    if (base_message_parser is IPeerMessageParser<TMessageData> message_parser)
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
        /// <typeparam name="TMessageData">Message data type</typeparam>
        /// <param name="messageParser">Message parser</param>
        /// <returns>"true" if message parser was successfully removed, otherwise "false"</returns>
        public bool RemoveMessageParser<TMessageData>(IPeerMessageParser<TMessageData> messageParser) where TMessageData : IBaseMessageData
        {
            if (messageParser == null)
            {
                throw new ArgumentNullException(nameof(messageParser));
            }
            bool ret = false;
            if (messageParsers.TryGetValue(messageParser.MessageType, out List<IBasePeerMessageParser> message_parsers))
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
        /// Sends an error message to peer asynchronously
        /// </summary>
        /// <typeparam name="TMessageData">Message data type</typeparam>
        /// <param name="peer">Peer</param>
        /// <param name="errorType">Error type</param>
        /// <param name="errorMessage">Error message</param>
        public Task SendErrorMessageToPeerAsync<TMessageData>(IPeer peer, EErrorType errorType, string errorMessage)
            where TMessageData : IBaseMessageData =>
            SendErrorMessageToPeerAsync<TMessageData>(peer, errorType, errorMessage, false);

        /// <summary>
        /// Sends an error message to peer asynchronously
        /// </summary>
        /// <typeparam name="TMessageData">Message data type</typeparam>
        /// <param name="peer">Peer</param>
        /// <param name="errorType">Error tyoe</param>
        /// <param name="errorMessage">Error message</param>
        /// <param name="isFatal">Is error fatal</param>
        /// <returns>Task</returns>
        public async Task SendErrorMessageToPeerAsync<TMessageData>(IPeer peer, EErrorType errorType, string errorMessage, bool isFatal)
            where TMessageData : IBaseMessageData
        {
            Console.Error.WriteLine($"[{ errorType }] { errorMessage }");
            await SendMessageToPeerAsync(peer, new ErrorMessageData(errorType, Naming.GetMessageTypeNameFromMessageDataType<TMessageData>(), errorMessage));
            if (isFatal)
            {
                peer.Disconnect(EDisconnectionReason.Error);
            }
        }

        /// <summary>
        /// Sends an invalid message parameters error message to peer asynchronously
        /// </summary>
        /// <typeparam name="TMessageData">Message data type</typeparam>
        /// <param name="peer">Peer</param>
        /// <param name="errorMessage">Error message</param>
        public Task SendInvalidMessageParametersErrorMessageToPeerAsync<TMessageData>(IPeer peer, string errorMessage)
            where TMessageData : IBaseMessageData =>
            SendErrorMessageToPeerAsync<TMessageData>(peer, EErrorType.InvalidMessageParameters, errorMessage);

        /// <summary>
        /// Sends an unknown error message to peer asynchronously
        /// </summary>
        /// <typeparam name="TMessageData">Message data type</typeparam>
        /// <param name="peer">Peer</param>
        /// <param name="errorMessage">Error message</param>
        /// <returns>Task</returns>
        public Task SendUnknownErrorMessageToPeerAsync<TMessageData>(IPeer peer, string errorMessage) where TMessageData : IBaseMessageData =>
            SendErrorMessageToPeerAsync<TMessageData>(peer, EErrorType.Unknown, errorMessage);

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
