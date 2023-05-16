namespace Taurus.Synchronizers
{
    /// <summary>
    /// Used to invoke when an user ping message has been received
    /// </summary>
    /// <typeparam name="TUser">User type</typeparam>
    /// <param name="user">Sending user</param>
    /// <param name="key">Key</param>
    /// <param name="latency">Latency</param>
    public delegate void UserPongMessageReceivedDelegate<TUser>(TUser user, int key, Latency latency) where TUser : IUser;
}
