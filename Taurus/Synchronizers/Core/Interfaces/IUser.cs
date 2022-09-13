using System;
using System.Threading.Tasks;
using Taurus.Connectors;
using Taurus.Serializers;

/// <summary>
/// Taurus synchronizers namespace
/// </summary>
namespace Taurus.Synchronizers
{
    /// <summary>
    /// An interface that represents an user
    /// </summary>
    public interface IUser
    {
        /// <summary>
        /// User GUID
        /// </summary>
        UserGUID UserGUID { get; }

        /// <summary>
        /// Peer
        /// </summary>
        IPeer Peer { get; }

        /// <summary>
        /// Serializer
        /// </summary>
        ISerializer Serializer { get; }

        /// <summary>
        /// Latency
        /// </summary>
        TimeSpan Latency { get; }

        /// <summary>
        /// Gets invoked when latency has changed
        /// </summary>
        event LatencyChangedDelegate? OnLatencyChanged;

        /// <summary>
        /// Sends a message to this user asynchronously
        /// </summary>
        /// <typeparam name="TMessageData">Message data type</typeparam>
        /// <param name="message">Message</param>
        /// <returns>Task</returns>
        Task SendMessageAsync<TMessageData>(TMessageData message) where TMessageData : IBaseMessageData;

        /// <summary>
        /// Sends an error message to this user asynchronously
        /// </summary>
        /// <param name="errorType">Error tyoe</param>
        /// <param name="errorMessage">Error message</param>
        /// <param name="errorMessageType">Error message type</param>
        /// <param name="isFatal">Is error fatal</param>
        /// <returns>Task</returns>
        Task SendErrorMessageAsync(EErrorType errorType, string errorMessage, string errorMessageType, bool isFatal);

        /// <summary>
        /// Sends an error message to this user asynchronously
        /// </summary>
        /// <param name="errorType">Error tyoe</param>
        /// <param name="errorMessage">Error message</param>
        /// <param name="errorMessageType">Error message type</param>
        /// <returns>Task</returns>
        Task SendErrorMessageAsync(EErrorType errorType, string errorMessage, string errorMessageType);

        /// <summary>
        /// Sends an error message to this user asynchronously
        /// </summary>
        /// <typeparam name="TMessageData">Message data type</typeparam>
        /// <param name="errorType">Error tyoe</param>
        /// <param name="errorMessage">Error message</param>
        /// <param name="isFatal">Is error fatal</param>
        /// <returns>Task</returns>
        Task SendErrorMessageAsync<TMessageData>(EErrorType errorType, string errorMessage, bool isFatal) where TMessageData : IBaseMessageData;

        /// <summary>
        /// Sends an error message to this user asynchronously
        /// </summary>
        /// <typeparam name="TMessageData">Message data type</typeparam>
        /// <param name="errorType">Error tyoe</param>
        /// <param name="errorMessage">Error message</param>
        /// <param name="isFatal">Is error fatal</param>
        /// <returns>Task</returns>
        Task SendErrorMessageAsync<TMessageData>(EErrorType errorType, string errorMessage) where TMessageData : IBaseMessageData;

        /// <summary>
        /// Sends a ping message to this user asynchronously
        /// </summary>
        /// <returns>Task</returns>
        Task SendPingMessageAsync();

        /// <summary>
        /// Receives pong message key
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>"true" if pong message key was awaited for, otherwise "false"</returns>
        bool ReceivePongMessageKey(int key);

        /// <summary>
        /// Disconnects this user
        /// </summary>
        /// <param name="disconnectionReason">Disconnection reason</param>
        void Disconnect(EDisconnectionReason disconnectionReason);
    }
}
