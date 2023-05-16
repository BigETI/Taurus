using System;
using Taurus.Serializers;

namespace Taurus.Synchronizers
{
    /// <summary>
    /// An interface that represents a base user message parser
    /// </summary>
    public interface IBaseUserMessageParser<TUser> where TUser : IUser
    {
        /// <summary>
        /// Message type
        /// </summary>
        string MessageType { get; }

        /// <summary>
        /// Serializer
        /// </summary>
        ISerializer Serializer { get; }

        /// <summary>
        /// Parses incoming user message
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="bytes">Bytes</param>
        void ParseUserMessage(TUser user, ReadOnlySpan<byte> bytes);
    }
}
