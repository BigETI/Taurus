using System;
using System.Threading.Tasks;

namespace Taurus.Synchronizers
{
    /// <summary>
    /// Used to invoke when parsing an user message has failed
    /// </summary>
    /// <typeparam name="TUser">User type</typeparam>
    /// <param name="user">Sending user</param>
    /// <param name="expectedMessageType">Expected message type</param>
    /// <param name="bytes">Message bytes</param>
    /// <returns>Task</returns>
    public delegate Task UserMessageParseFailedDelegate<TUser>(TUser user, string expectedMessageType, ReadOnlyMemory<byte> bytes) where TUser : IUser;
}
