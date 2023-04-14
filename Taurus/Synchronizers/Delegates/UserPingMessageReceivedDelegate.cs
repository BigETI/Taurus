/// <summary>
/// Taurus synchronizers namespace
/// </summary>
namespace Taurus.Synchronizers
{
    /// <summary>
    /// Used to invoke when an user ping message has been received
    /// </summary>
    /// <typeparam name="TUser">User type</typeparam>
    /// <param name="user">Sending user</param>
    /// <param name="key">Key</param>
    public delegate void UserPingMessageReceivedDelegate<TUser>(TUser user, int key) where TUser : IUser;
}
