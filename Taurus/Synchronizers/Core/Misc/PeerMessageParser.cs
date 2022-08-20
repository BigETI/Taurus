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
    /// <typeparam name="TMessageData">Message data type</typeparam>
    internal class PeerMessageParser<TMessageData> : IPeerMessageParser<TMessageData> where TMessageData : IBaseMessageData
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
        public event PeerMessageParsedDelegate<TMessageData> OnPeerMessageParsed;

        /// <summary>
        /// Gets invoked when validating a peer message has failed
        /// </summary>
        public event PeerMessageValidationFailedDelegate<TMessageData> OnPeerMessageValidationFailed;

        /// <summary>
        /// Gets invoked when parsing a peer message has failed
        /// </summary>
        public event PeerMessageParseFailedDelegate OnPeerMessageParseFailed;

        /// <summary>
        /// Constructs a new message parser
        /// </summary>
        /// <param name="onPeerMessageParsed">On peer message parsed</param>
        /// <param name="onPeerMessageValidationFailed">On message validation failed</param>
        /// <param name="onPeerMessageParseFailed">On peer message parse failed</param>
        public PeerMessageParser(ISerializer serializer, PeerMessageParsedDelegate<TMessageData> onPeerMessageParsed, PeerMessageValidationFailedDelegate<TMessageData> onPeerMessageValidationFailed, PeerMessageParseFailedDelegate onPeerMessageParseFailed)
        {
            MessageType = Naming.GetMessageTypeNameFromMessageDataType<TMessageData>();
            Serializer = serializer;
            OnPeerMessageParsed += onPeerMessageParsed;
            OnPeerMessageValidationFailed += onPeerMessageValidationFailed;
            OnPeerMessageParseFailed += onPeerMessageParseFailed;
        }

        /// <summary>
        /// Parses incoming message
        /// </summary>
        /// <param name="peer">Peer</param>
        /// <param name="bytes">Bytes</param>
        public void ParseMessage(IPeer peer, ReadOnlySpan<byte> bytes)
        {
            TMessageData message = Serializer.Deserialize<TMessageData>(bytes);
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
