using System;

/// <summary>
/// Taurus synchronizers namespace
/// </summary>
namespace Taurus.Synchronizers
{
    /// <summary>
    /// An interface that represents a base user message parser
    /// </summary>
    public interface IBaseUserMessageParser<TUser, TSynchronizer>
        where TUser : IUser<TUser, TSynchronizer>
        where TSynchronizer : ISynchronizer<TSynchronizer, TUser>
    {
        /// <summary>
        /// Message type
        /// </summary>
        string MessageType { get; }

        /// <summary>
        /// Synchronizer
        /// </summary>
        TSynchronizer Synchronizer { get; }

        /// <summary>
        /// Parses incoming user message
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="bytes">Bytes</param>
        void ParseUserMessage(TUser user, ReadOnlySpan<byte> bytes);
    }
}
