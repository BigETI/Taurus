namespace Taurus.Synchronizers
{
    /// <summary>
    /// Used to invoke when an user error message has been received
    /// </summary>
    /// <typeparam name="TUser">User type</typeparam>
    /// <param name="user">Sending user</param>
    /// <param name="errorType">Error type</param>
    /// <param name="issuingMessageType">Issuing message type</param>
    /// <param name="message">Error message</param>
    public delegate void UserErrorMessageReceivedDelegate<TUser>
    (
        TUser user,
        EErrorType errorType,
        string issuingMessageType,
        string message
    ) where TUser : IUser;
}
