using System;
using System.Threading.Tasks;

/// <summary>
/// Taurus synchronizers authenticated users namespace
/// </summary>
namespace Taurus.Synchronizers.AuthenticatedUsers
{
    /// <summary>
    /// Used to invoke when an authenticated user message validation has failed
    /// </summary>
    /// <typeparam name="TUser">User type</typeparam>
    /// <typeparam name="TMessageData">Message data tyoe</typeparam>
    /// <param name="user">User</param>
    /// <param name="message">Message</param>
    /// <param name="bytes">Message bytes</param>
    /// <returns>Task</returns>
    public delegate Task AuthenticatedUserMessageValidationFailedDelegate<TUser, TMessageData>(TUser user, TMessageData message, ReadOnlyMemory<byte> bytes)
        where TUser : IUser
        where TMessageData : IBaseMessageData;
}
