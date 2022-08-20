using System;
using System.Threading.Tasks;

/// <summary>
/// Taurus synchronizers namespace
/// </summary>
namespace Taurus.Synchronizers
{
    /// <summary>
    /// Used to invoke when parsing an authenticated user message has failed
    /// </summary>
    /// <typeparam name="TUser">User type</typeparam>
    /// <param name="user">Sending user</param>
    /// <param name="expectedMessageType">Expected message type</param>
    /// <param name="bytes">Message bytes</param>
    /// <returns>Task</returns>
    public delegate Task AuthenticatedUserMessageParseFailedDelegate<TUser>(TUser user, string expectedMessageType, ReadOnlyMemory<byte> bytes)
        where TUser : IUser;
}
