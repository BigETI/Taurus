namespace Taurus.Synchronizers
{
    /// <summary>
    /// USed to invoke when a message needs to be sent to an user
    /// </summary>
    /// <typeparam name="TUser">User type</typeparam>
    /// <param name="user">User</param>
    /// <returns>"true" if message should be sent to this user, otherwise "false"</returns>
    public delegate bool SendMessageToUserDelegate<TUser>(TUser user) where TUser : IUser;
}
