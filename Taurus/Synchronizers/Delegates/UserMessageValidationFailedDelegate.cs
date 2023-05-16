using System;
using System.Threading.Tasks;

namespace Taurus.Synchronizers
{
    /// <summary>
    /// Used to invoke when an user message validation has failed
    /// </summary>
    /// <typeparam name="TUser">User type</typeparam>
    /// <typeparam name="TMessageData">Message data tyoe</typeparam>
    /// <param name="user">Sending user</param>
    /// <param name="message">Message</param>
    /// <param name="bytes">Message bytes</param>
    /// <returns>Task</returns>
    public delegate Task UserMessageValidationFailedDelegate<TUser, TMessageData>(TUser user, TMessageData message, ReadOnlyMemory<byte> bytes)
        where TUser : IUser
        where TMessageData : IBaseMessageData;
}
