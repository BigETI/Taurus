using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Taurus.Connectors;
using Taurus.Serializers;

namespace Taurus.Synchronizers
{
    /// <summary>
    /// An interface that represents a synchronizer
    /// </summary>
    /// <typeparam name="TUser">User type</typeparam>
    public interface ISynchronizer<TUser> :
        IProcessableEvents,
        IDisposable
        where TUser : IUser
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
        /// Users
        /// </summary>
        IReadOnlyDictionary<UserGUID, TUser> Users { get; }

        /// <summary>
        /// Gets invoked when a peer connection has been attempted
        /// </summary>
        event PeerConnectionAttemptedDelegate? OnPeerConnectionAttempted;

        /// <summary>
        /// Gets invoked when a peer has been connected
        /// </summary>
        event PeerConnectedDelegate? OnPeerConnected;

        /// <summary>
        /// Gets invoked when a peer has been disconnected
        /// </summary>
        event PeerDisconnectedDelegate? OnPeerDisconnected;

        /// <summary>
        /// Gets invoked when a peer message has been received
        /// </summary>
        event PeerMessageReceivedDelegate? OnPeerMessageReceived;

        /// <summary>
        /// Gets invoked when an user has been connected
        /// </summary>
        event UserConnectedDelegate<TUser>? OnUserConnected;

        /// <summary>
        /// Gets invoked when an user has been disconnected
        /// </summary>
        event UserDisconnectedDelegate<TUser>? OnUserDisconnected;

        /// <summary>
        /// Gets invoked when an unknown user message has been received
        /// </summary>
        event UnknownUserMessageReceivedDelegate<TUser>? OnUnknownUserMessageReceived;

        /// <summary>
        /// Gets invoked when an user error message has been received
        /// </summary>
        event UserErrorMessageReceivedDelegate<TUser>? OnUserErrorMessageReceived;

        /// <summary>
        /// Gets invoked when an user ping message has been received
        /// </summary>
        event UserPingMessageReceivedDelegate<TUser>? OnUserPingMessageReceived;

        /// <summary>
        /// Gets invoked when an user pong message has been received
        /// </summary>
        event UserPongMessageReceivedDelegate<TUser>? OnUserPongMessageReceived;

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
        bool TryGettingConnectorOfType<TConnector>(out TConnector connector) where TConnector : IConnector?;

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
        /// Adds a new user message parser
        /// </summary>
        /// <typeparam name="TMessageData">Message data type</typeparam>
        /// <param name="onUserMessageParsed">Gets invoked when a peer message has been parsed</param>
        /// <param name="onUserMessageValidationFailed">Gets invoked when validating a peer message has failed</param>
        /// <param name="onUserMessageParseFailed">Gets invoked when parsing a peer message has failed</param>
        /// <returns>Message parser</returns>
        IUserMessageParser<TUser, TMessageData> AddNewUserMessageParser<TMessageData>
        (
            UserMessageParsedDelegate<TUser, TMessageData> onUserMessageParsed,
            UserMessageValidationFailedDelegate<TUser, TMessageData> onUserMessageValidationFailed,
            UserMessageParseFailedDelegate<TUser> onUserMessageParseFailed
        ) where TMessageData : IBaseMessageData;

        /// <summary>
        /// Adds a new automatic user message parser
        /// </summary>
        /// <typeparam name="TMessageData">Message data type</typeparam>
        /// <param name="onUserMessageParsed">Gets invoked when an user message has been parsed</param>
        /// <param name="isFatal">Is validation fail or error fatal</param>
        /// <returns>Message parser</returns>
        IUserMessageParser<TUser, TMessageData> AddNewAutomaticUserMessageParser<TMessageData>
        (
            UserMessageParsedDelegate<TUser, TMessageData> onUserMessageParsed,
            bool isFatal
        ) where TMessageData : IBaseMessageData;

        /// <summary>
        /// Adds a new automatic user message parser
        /// </summary>
        /// <typeparam name="TMessageData">Message data type</typeparam>
        /// <param name="onUserMessageParsed">Gets invoked when an user message has been parsed</param>
        /// <returns>Message parser</returns>
        IUserMessageParser<TUser, TMessageData> AddNewAutomaticUserMessageParser<TMessageData>
        (
            UserMessageParsedDelegate<TUser, TMessageData> onUserMessageParsed
        ) where TMessageData : IBaseMessageData;

        /// <summary>
        /// Adds a new automatic user message parser that is fatal on validation fail or error
        /// </summary>
        /// <typeparam name="TMessageData">Message data type</typeparam>
        /// <param name="onUserMessageParsed">Gets invoked when an user message has been parsed</param>
        /// <returns>Message parser</returns>
        IUserMessageParser<TUser, TMessageData> AddNewAutomaticUserMessageParserWithFatality<TMessageData>
        (
            UserMessageParsedDelegate<TUser, TMessageData> onUserMessageParsed
        ) where TMessageData : IBaseMessageData;

        /// <summary>
        /// Gets user message parsers for the specified type
        /// </summary>
        /// <typeparam name="TMessageData">Message data type</typeparam>
        /// <returns>Message parsers if successful, otherwise "null"</returns>
        IEnumerable<IUserMessageParser<TUser, TMessageData>> GetUserMessageParsersForType<TMessageData>()
            where TMessageData : IBaseMessageData;

        /// <summary>
        /// Tries to get user message parsers for the specified type
        /// </summary>
        /// <typeparam name="TMessageData">Message data type</typeparam>
        /// <param name="userMessageParsers">User message parsers</param>
        /// <returns>"true" if message parsers are available, otherwise "false"</returns>
        bool TryGettingUserMessageParsersForType<TMessageData>
        (
            out IEnumerable<IUserMessageParser<TUser, TMessageData>>? userMessageParsers
        ) where TMessageData : IBaseMessageData;

        /// <summary>
        /// Removes the specified user message parser
        /// </summary>
        /// <typeparam name="TMessageData">Message data type</typeparam>
        /// <param name="userMessageParser">User message parser</param>
        /// <returns>"true" if message parser was successfully removed, otherwise "false"</returns>
        bool RemoveUserMessageParser<TMessageData>(IUserMessageParser<TUser, TMessageData> userMessageParser)
            where TMessageData : IBaseMessageData;

        /// <summary>
        /// Broadcasts the specified message
        /// </summary>
        /// <typeparam name="TMessageData">Message data</typeparam>
        /// <param name="message">Message</param>
        /// <returns>Task</returns>
        Task BroadcastMessageAsync<TMessageData>(TMessageData message) where TMessageData : IBaseMessageData;

        /// <summary>
        /// Broadcasts the specified message
        /// </summary>
        /// <typeparam name="TMessageData">Message data type</typeparam>
        /// <param name="message">Message</param>
        /// <param name="onSendMessageToUser">Gets invoked when a message needs to be sent to an user</param>
        /// <returns>Task</returns>
        Task BroadcastMessageAsync<TMessageData>(TMessageData message, SendMessageToUserDelegate<TUser> onSendMessageToUser)
            where TMessageData : IBaseMessageData;

        /// <summary>
        /// Closes connections to all peers
        /// </summary>
        /// <param name="reason">Disconnection reason</param>
        void Close(EDisconnectionReason reason);
    }
}
