/// <summary>
/// Taurus synchronizers namespace
/// </summary>
namespace Taurus.Synchronizers
{
    /// <summary>
    /// Used to invoke when an user error message has been received
    /// </summary>
    /// <typeparam name="TUser">User type</typeparam>
    /// <typeparam name="TSynchronizer">Synchronizer type</typeparam>
    /// <param name="user">Sending user</param>
    /// <param name="errorType">Error type</param>
    /// <param name="issuingMessageType">Issuing message type</param>
    /// <param name="message">Error message</param>
    public delegate void UserErrorMessageReceivedDelegate<TUser, TSynchronizer>
    (
        TUser user,
        EErrorType errorType,
        string issuingMessageType,
        string message
    )
        where TUser : IUser<TUser, TSynchronizer>
        where TSynchronizer : ISynchronizer<TSynchronizer, TUser>;
}
