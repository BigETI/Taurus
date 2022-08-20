using System;
using System.Threading.Tasks;

/// <summary>
/// Taurus synchronizers namespace
/// </summary>
namespace Taurus.Synchronizers
{
    /// <summary>
    /// Used to invoke when an authenticated user message has been parsed
    /// </summary>
    /// <typeparam name="TUser">User type</typeparam>
    /// <typeparam name="TMessageData">Message data type</typeparam>
    /// <param name="user">User</param>
    /// <param name="message">Message</param>
    /// <param name="bytes">Bytes</param>
    /// <returns>Task</returns>
    public delegate Task AuthenticatedUserMessageParsedDelegate<TUser, TMessageData>(TUser user, TMessageData message, ReadOnlyMemory<byte> bytes)
        where TUser : IUser
        where TMessageData : IBaseMessageData;
}
