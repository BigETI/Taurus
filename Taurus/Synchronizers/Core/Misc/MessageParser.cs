using System;
using Taurus.Connectors;
using Taurus.Serializers;

namespace Taurus.Synchronizers
{
    /// <summary>
    /// Message parser class
    /// </summary>
    /// <typeparam name="T">Message type</typeparam>
    internal class MessageParser<T> : IMessageParser<T> where T : IBaseMessageData
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
        /// On message parsed
        /// </summary>
        public event MessageParsedDelegate<T> OnMessageParsed;

        /// <summary>
        /// On message validation failed
        /// </summary>
        public event MessageValidationFailedDelegate<T>? OnMessageValidationFailed;

        /// <summary>
        /// On message parse failed
        /// </summary>
        public event MessageParseFailedDelegate? OnMessageParseFailed;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="onMessageParsed">On message parsed</param>
        /// <param name="onMessageValidationFailed">On message validation failed</param>
        /// <param name="onMessageParseFailed">On message parse failed</param>
        public MessageParser(ISerializer serializer, MessageParsedDelegate<T> onMessageParsed, MessageValidationFailedDelegate<T>? onMessageValidationFailed, MessageParseFailedDelegate? onMessageParseFailed)
        {
            MessageType = Naming.GetMessageTypeNameFromMessageDataType<T>();
            Serializer = serializer;
            OnMessageParsed += onMessageParsed ?? throw new ArgumentNullException(nameof(onMessageParsed));
            if (onMessageValidationFailed != null)
            {
                OnMessageValidationFailed += onMessageValidationFailed;
            }
            if (onMessageParseFailed != null)
            {
                OnMessageParseFailed += onMessageParseFailed;
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
                    OnMessageParseFailed?.Invoke(peer, MessageType, bytes);
                }
                if (message.IsValid)
                {
                    OnMessageParsed?.Invoke(peer, message, bytes);
                }
                else
                {
                    OnMessageValidationFailed?.Invoke(peer, message, bytes);
                }
            }
            else
            {
                OnMessageParseFailed?.Invoke(peer, MessageType, bytes);
            }
        }
    }
}
