using System;
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
    /// An interface that represents a synchronizer
    /// </summary>
    public interface ISynchronizer : IProcessableEvents, IDisposable
    {
        /// <summary>
        /// Available connectors
        /// </summary>
        IEnumerable<IConnector> Connectors { get; }

        /// <summary>
        /// Serializer
        /// </summary>
        ISerializer Serializer { get; }

        /// <summary>
        /// Gets invoked when a peer connection has been attempted
        /// </summary>
        event PeerConnectionAttemptedDelegate OnPeerConnectionAttempted;

        /// <summary>
        /// Gets invoked when a peer has been connected
        /// </summary>
        event PeerConnectedDelegate OnPeerConnected;

        /// <summary>
        /// Gets invoked when a peer has been disconnected
        /// </summary>
        event PeerDisconnectedDelegate OnPeerDisconnected;

        /// <summary>
        /// Gets invoked when a peer message has been received
        /// </summary>
        event PeerMessageReceivedDelegate OnPeerMessageReceived;

        /// <summary>
        /// Gets invoked when an unknown peer message has been received
        /// </summary>
        event UnknownPeerMessageReceivedDelegate OnUnknownPeerMessageReceived;

        /// <summary>
        /// Gets invoked when an peer error message has been received
        /// </summary>
        event PeerErrorMessageReceivedDelegate OnPeerErrorMessageReceived;

        /// <summary>
        /// Gets invoked when a peer ping message has been received
        /// </summary>
        event PeerPingMessageReceivedDelegate? OnPeerPingMessageReceived;

        /// <summary>
        /// Gets invoked when a peer pong message has been received
        /// </summary>
        event PeerPongMessageReceivedDelegate? OnPeerPongMessageReceived;

        /// <summary>
        /// Adds the specified connector
        /// </summary>
        /// <param name="connector">Connector</param>
        /// <returns>"true" if the specified connector was successfully added, otherwise "false"</returns>
        bool AddConnector(IConnector connector);

        /// <summary>
        /// Removes the specified connector
        /// </summary>
        /// <param name="connector">Connector</param>
        /// <returns>"true" if the specified connector was successfully removed, otherwise "false"</returns>
        bool RemoveConnector(IConnector connector);

        /// <summary>
        /// Gets a connector with the specified type
        /// </summary>
        /// <typeparam name="TConnector">Connector type</typeparam>
        /// <returns>Connector of specified type if successful, otherwise "null"</returns>
        TConnector GetConnectorOfType<TConnector>() where TConnector : IConnector?;

        /// <summary>
        /// Tries to get a connector of the specified type
        /// </summary>
        /// <typeparam name="TConnector">Connector type</typeparam>
        /// <param name="connector">Connector</param>
        /// <returns>"true" if connector of the specified type is available, otherwise "false"</returns>
        bool TryGetConnectorOfType<TConnector>(out TConnector connector) where TConnector : IConnector?;

        /// <summary>
        /// Sends a message to peer asynchronously
        /// </summary>
        /// <typeparam name="TMessageData">Message data type</typeparam>
        /// <param name="peer">Peer</param>
        /// <param name="message">Message</param>
        /// <returns>Task</returns>
        Task SendMessageToPeerAsync<TMessageData>(IPeer peer, TMessageData message) where TMessageData : IBaseMessageData;

        /// <summary>
        /// Adds a peer message parser
        /// </summary>
        /// <typeparam name="TMessageData">Message data type</typeparam>
        /// <param name="onPeerMessageParsed">On peer message parsed</param>
        /// <param name="onPeerMessageValidationFailed">On message validation failed</param>
        /// <param name="onMessageParseFailed">On message parse failed</param>
        /// <returns>Message parser</returns>
        IPeerMessageParser<TMessageData> AddPeerMessageParser<TMessageData>(PeerMessageParsedDelegate<TMessageData> onPeerMessageParsed, PeerMessageValidationFailedDelegate<TMessageData> onPeerMessageValidationFailed, PeerMessageParseFailedDelegate onMessageParseFailed)
            where TMessageData : IBaseMessageData;

        /// <summary>
        /// Adds an automatic peer message parser
        /// </summary>
        /// <typeparam name="TMessageData">Message data type</typeparam>
        /// <param name="onPeerMessageParsed">Gets invoked when a peer message has been parsed</param>
        /// <param name="isFatal">Is validation fail or error fatal</param>
        /// <returns>Message parser</returns>
        IPeerMessageParser<TMessageData> AddAutomaticPeerMessageParser<TMessageData>(PeerMessageParsedDelegate<TMessageData> onPeerMessageParsed, bool isFatal)
            where TMessageData : IBaseMessageData;

        /// <summary>
        /// Adds an automatic peer message parser
        /// </summary>
        /// <typeparam name="TMessageData">Message data type</typeparam>
        /// <param name="onPeerMessageParsed">Gets invoked when a peer message has been parsed</param>
        /// <returns>Message parser</returns>
        IPeerMessageParser<TMessageData> AddAutomaticPeerMessageParser<TMessageData>(PeerMessageParsedDelegate<TMessageData> onPeerMessageParsed)
            where TMessageData : IBaseMessageData;

        /// <summary>
        /// Adds an automatic peer message parser that is fatal on validation fail or error
        /// </summary>
        /// <typeparam name="TMessageData">Message data type</typeparam>
        /// <param name="onPeerMessageParsed">Gets invoked when a peer message has been parsed</param>
        /// <returns>Message parser</returns>
        IPeerMessageParser<TMessageData> AddAutomaticPeerMessageParserWithFatality<TMessageData>(PeerMessageParsedDelegate<TMessageData> onPeerMessageParsed)
            where TMessageData : IBaseMessageData;

        /// <summary>
        /// Gets message parsers for the specified type
        /// </summary>
        /// <typeparam name="TMessageData">Message data type</typeparam>
        /// <returns>Message parsers if successful, otherwise "null"</returns>
        IEnumerable<IPeerMessageParser<TMessageData>> GetPeerMessageParsersForType<TMessageData>() where TMessageData : IBaseMessageData;

        /// <summary>
        /// Tries to get message parsers for the specified type
        /// </summary>
        /// <typeparam name="TMessageData">Message data type</typeparam>
        /// <param name="messageParsers">Message parsers</param>
        /// <returns>"true" if message parsers are available, otherwise "false"</returns>
        bool TryGetMessageParsersForType<TMessageData>(out IEnumerable<IPeerMessageParser<TMessageData>>? messageParsers)
            where TMessageData : IBaseMessageData;

        /// <summary>
        /// Removes the specified message parser
        /// </summary>
        /// <typeparam name="TMessageData">Message data type</typeparam>
        /// <param name="messageParser">Message parser</param>
        /// <returns>"true" if message parser was successfully removed, otherwise "false"</returns>
        bool RemoveMessageParser<TMessageData>(IPeerMessageParser<TMessageData> messageParser) where TMessageData : IBaseMessageData;

        /// <summary>
        /// Sends an invalid message parameters error message to the specified peer asynchronously
        /// </summary>
        /// <typeparam name="TMessageData">Message data type</typeparam>
        /// <param name="peer">Peer</param>
        /// <param name="errorMessage">Error message</param>
        /// <returns>TAsk</returns>
        Task SendInvalidMessageParametersErrorMessageToPeerAsync<TMessageData>(IPeer peer, string errorMessage) where TMessageData : IBaseMessageData;

        /// <summary>
        /// Sends an unknown error message to the specified peer
        /// </summary>
        /// <typeparam name="TMessageData">Message data type</typeparam>
        /// <param name="peer">Peer</param>
        /// <param name="errorMessage">Error message</param>
        /// <returns>Task</returns>
        Task SendUnknownErrorMessageToPeerAsync<TMessageData>(IPeer peer, string errorMessage) where TMessageData : IBaseMessageData;

        /// <summary>
        /// Send a ping message to the specified peer
        /// </summary>
        /// <param name="peer">Peer</param>
        /// <returns>Task</returns>
        Task SendPeerPingMessageAsync(IPeer peer);

        /// <summary>
        /// Closes connections to all peers
        /// </summary>
        /// <param name="reason">Disconnection reason</param>
        void Close(EDisconnectionReason reason);
    }
}
