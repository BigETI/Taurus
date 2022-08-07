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
    public interface ISynchronizer : ISynchronizedEvents, IDisposable
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
        /// Gets invoked when an user has been authenticated
        /// </summary>
        event UserAuthenticatedDelegate? OnUserAuthenticated;

        /// <summary>
        /// Gets invoked when an user has been disconnected
        /// </summary>
        event UserDisconnectedDelegate? OnUserDisconnected;

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
        /// <typeparam name="T">Connector type</typeparam>
        /// <returns>Connector of specified type if successful, otherwise "null"</returns>
        T GetConnectorOfType<T>() where T : IConnector?;

        /// <summary>
        /// Tries to get a connector of the specified type
        /// </summary>
        /// <typeparam name="T">Connector type</typeparam>
        /// <param name="connector">Connector</param>
        /// <returns>"true" if connector of the specified type is available, otherwise "false"</returns>
        bool TryGetConnectorOfType<T>(out T connector) where T : IConnector?;

        /// <summary>
        /// Sends a message to peer asynchronously
        /// </summary>
        /// <typeparam name="T">Message type</typeparam>
        /// <param name="peer">Peer</param>
        /// <param name="message">Message</param>
        /// <returns>Task</returns>
        Task SendMessageToPeerAsync<T>(IPeer peer, T message) where T : IBaseMessageData;

        /// <summary>
        /// Adds a message parser
        /// </summary>
        /// <typeparam name="T">Message type</typeparam>
        /// <param name="onPeerMessageParsed">On peer message parsed</param>
        /// <param name="onPeerMessageValidationFailed">On message validation failed</param>
        /// <param name="onMessageParseFailed">On message parse failed</param>
        /// <returns>Message parser</returns>
        IPeerMessageParser<T> AddMessageParser<T>(PeerMessageParsedDelegate<T> onPeerMessageParsed, PeerMessageValidationFailedDelegate<T>? onPeerMessageValidationFailed = null, PeerMessageParseFailedDelegate? onMessageParseFailed = null) where T : IBaseMessageData;

        /// <summary>
        /// Gets message parsers for the specified type
        /// </summary>
        /// <typeparam name="T">Message type</typeparam>
        /// <returns>Message parsers if successful, otherwise "null"</returns>
        IEnumerable<IPeerMessageParser<T>> GetMessageParsersForType<T>() where T : IBaseMessageData;

        /// <summary>
        /// Tries to get message parsers for the specified type
        /// </summary>
        /// <typeparam name="T">Message type</typeparam>
        /// <param name="messageParsers">Message parsers</param>
        /// <returns>"true" if message parsers are available, otherwise "false"</returns>
        bool TryGetMessageParsersForType<T>(out IEnumerable<IPeerMessageParser<T>>? messageParsers) where T : IBaseMessageData;

        /// <summary>
        /// Removes the specified message parser
        /// </summary>
        /// <typeparam name="T">Message type</typeparam>
        /// <param name="messageParser">Message parser</param>
        /// <returns>"true" if message parser was successfully removed, otherwise "false"</returns>
        bool RemoveMessageParser<T>(IPeerMessageParser<T> messageParser) where T : IBaseMessageData;

        /// <summary>
        /// Sends an invalid message parameters error message to peer asynchronously
        /// </summary>
        /// <typeparam name="T">Message type</typeparam>
        /// <param name="peer">Peer</param>
        /// <param name="errorMessage">Error message</param>
        /// <returns>TAsk</returns>
        Task SendInvalidMessageParametersErrorMessageToPeerAsync<T>(IPeer peer, string errorMessage) where T : IBaseMessageData;

        /// <summary>
        /// Sends an invalid message context error message to peer asynchronously
        /// </summary>
        /// <typeparam name="T">Message type</typeparam>
        /// <param name="peer">Peer</param>
        /// <param name="errorMessage">Error message</param>
        /// <returns>Task</returns>
        Task SendInvalidMessageContextErrorMessageToPeerAsync<T>(IPeer peer, string errorMessage) where T : IBaseMessageData;

        /// <summary>
        /// Sends an unknown error message to peer
        /// </summary>
        /// <typeparam name="T">Message type</typeparam>
        /// <param name="peer">Peer</param>
        /// <param name="errorMessage">Error message</param>
        /// <returns>Task</returns>
        Task SendUnknownErrorMessageToPeerAsync<T>(IPeer peer, string errorMessage) where T : IBaseMessageData;

        /// <summary>
        /// Closes connections to all peers
        /// </summary>
        /// <param name="reason">Disconnection reason</param>
        void Close(EDisconnectionReason reason);
    }
}
