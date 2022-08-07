using System;
using Taurus.Connectors;
using Taurus.Serializers;

/// <summary>
/// Taurus synchronizers namespace
/// </summary>
namespace Taurus.Synchronizers
{
    /// <summary>
    /// A class that describes a peer message parser
    /// </summary>
    /// <typeparam name="T">Message type</typeparam>
    internal class PeerMessageParser<T> : IPeerMessageParser<T> where T : IBaseMessageData
    {
        /// <summary>
        /// Message type
        /// </summary>
        public string MessageType { get; }

        /// <summary>
        /// Serializer
        /// </summary>
        public ISerializer Serializer { get; }

        /// <summary>
        /// Gets invoked when a peer message has been parsed
        /// </summary>
        public event PeerMessageParsedDelegate<T> OnPeerMessageParsed;

        /// <summary>
        /// Gets invoked when validating a peer message has failed
        /// </summary>
        public event PeerMessageValidationFailedDelegate<T>? OnPeerMessageValidationFailed;

        /// <summary>
        /// Gets invoked when parsing a peer message has failed
        /// </summary>
        public event PeerMessageParseFailedDelegate? OnPeerMessageParseFailed;

        /// <summary>
        /// Constructs a new message parser
        /// </summary>
        /// <param name="onPeerMessageParsed">On peer message parsed</param>
        /// <param name="onPeerMessageValidationFailed">On message validation failed</param>
        /// <param name="onMessageParseFailed">On message parse failed</param>
        public PeerMessageParser(ISerializer serializer, PeerMessageParsedDelegate<T> onPeerMessageParsed, PeerMessageValidationFailedDelegate<T>? onPeerMessageValidationFailed, PeerMessageParseFailedDelegate? onMessageParseFailed)
        {
            MessageType = Naming.GetMessageTypeNameFromMessageDataType<T>();
            Serializer = serializer;
            OnPeerMessageParsed += onPeerMessageParsed ?? throw new ArgumentNullException(nameof(onPeerMessageParsed));
            if (onPeerMessageValidationFailed != null)
            {
                OnPeerMessageValidationFailed += onPeerMessageValidationFailed;
            }
            if (onMessageParseFailed != null)
            {
                OnPeerMessageParseFailed += onMessageParseFailed;
            }
        }

        /// <summary>
        /// Parses incoming message
        /// </summary>
        /// <param name="peer">Peer</param>
        /// <param name="bytes">Bytes</param>
        public void ParseMessage(IPeer peer, ReadOnlySpan<byte> bytes)
        {
            T message = Serializer.Deserialize<T>(bytes);
            if (message != null)
            {
                if (message.MessageType != MessageType)
                {
                    OnPeerMessageParseFailed?.Invoke(peer, MessageType, bytes.ToArray());
                }
                if (message.IsValid)
                {
                    OnPeerMessageParsed?.Invoke(peer, message, bytes.ToArray());
                }
                else
                {
                    OnPeerMessageValidationFailed?.Invoke(peer, message, bytes.ToArray());
                }
            }
            else
            {
                OnPeerMessageParseFailed?.Invoke(peer, MessageType, bytes.ToArray());
            }
        }
    }
}
