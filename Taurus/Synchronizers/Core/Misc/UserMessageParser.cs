using System;
using Taurus.Serializers;

/// <summary>
/// Taurus synchronizers namespace
/// </summary>
namespace Taurus.Synchronizers
{
    /// <summary>
    /// A class that describes an user message parser
    /// </summary>
    /// <typeparam name="TUser">User type</typeparam>
    /// <typeparam name="TSynchronizer">Synchronizer type</typeparam>
    /// <typeparam name="TMessageData">Message data type</typeparam>
    internal class UserMessageParser<TUser, TMessageData> : IUserMessageParser<TUser, TMessageData>
        where TUser : IUser
        where TMessageData : IBaseMessageData
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
        /// Gets invoked when an user message has been parsed
        /// </summary>
        public event UserMessageParsedDelegate<TUser, TMessageData> OnUserMessageParsed;

        /// <summary>
        /// Gets invoked when validating an user message has failed
        /// </summary>
        public event UserMessageValidationFailedDelegate<TUser, TMessageData> OnUserMessageValidationFailed;

        /// <summary>
        /// Gets invoked when parsing an user message has failed
        /// </summary>
        public event UserMessageParseFailedDelegate<TUser> OnUserMessageParseFailed;

        /// <summary>
        /// Constructs a new message parser
        /// </summary>
        /// <param name="serializer">Serializer</param>
        /// <param name="onPeerMessageParsed">On peer message parsed</param>
        /// <param name="onPeerMessageValidationFailed">On message validation failed</param>
        /// <param name="onPeerMessageParseFailed">On peer message parse failed</param>
        public UserMessageParser
        (
            ISerializer serializer,
            UserMessageParsedDelegate<TUser, TMessageData> onPeerMessageParsed,
            UserMessageValidationFailedDelegate<TUser, TMessageData> onPeerMessageValidationFailed,
            UserMessageParseFailedDelegate<TUser> onPeerMessageParseFailed
        )
        {
            MessageType = Naming.GetMessageTypeNameFromMessageDataType<TMessageData>();
            Serializer = serializer;
            OnUserMessageParsed += onPeerMessageParsed;
            OnUserMessageValidationFailed += onPeerMessageValidationFailed;
            OnUserMessageParseFailed += onPeerMessageParseFailed;
        }

        /// <summary>
        /// Parses incoming user message
        /// </summary>
        /// <param name="peer">Peer</param>
        /// <param name="bytes">Bytes</param>
        public void ParseUserMessage(TUser user, ReadOnlySpan<byte> bytes)
        {
            TMessageData message = Serializer.Deserialize<TMessageData>(bytes);
            if (message != null)
            {
                if (message.MessageType != MessageType)
                {
                    OnUserMessageParseFailed?.Invoke(user, MessageType, bytes.ToArray());
                }
                if (message.IsValid)
                {
                    OnUserMessageParsed?.Invoke(user, message, bytes.ToArray());
                }
                else
                {
                    OnUserMessageValidationFailed?.Invoke(user, message, bytes.ToArray());
                }
            }
            else
            {
                OnUserMessageParseFailed?.Invoke(user, MessageType, bytes.ToArray());
            }
        }
    }
}
