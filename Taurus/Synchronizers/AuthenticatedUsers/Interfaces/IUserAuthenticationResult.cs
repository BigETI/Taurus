/// <summary>
/// Taurus synchronizers authenticated users namespace
/// </summary>
namespace Taurus.Synchronizers.AuthenticatedUsers
{
    /// <summary>
    /// An interface that represents an user authentication result
    /// </summary>
    /// <typeparam name="TUser">User type</typeparam>
    /// <typeparam name="TAuthenticationSuccessfulMessageData">AUthentication successful message data type</typeparam>
    /// <typeparam name="TAuthenticationFailedMessageData">Authentication failed message data type</typeparam>
    public interface IUserAuthenticationResult<TUser, TAuthenticationSuccessfulMessageData, TAuthenticationFailedMessageData>
        where TUser : class, IUser
        where TAuthenticationSuccessfulMessageData : class, IBaseMessageData
        where TAuthenticationFailedMessageData : class, IBaseMessageData
    {
        /// <summary>
        /// Is user authentication successful
        /// </summary>
        bool IsSuccessful { get; }

        /// <summary>
        /// User
        /// </summary>
        TUser? User { get; }

        /// <summary>
        /// Authentication successful message data
        /// </summary>
        public TAuthenticationSuccessfulMessageData? AuthenticationSuccessfulMessageData { get; }

        /// <summary>
        /// Authentication failed message data
        /// </summary>
        public TAuthenticationFailedMessageData? AuthenticationFailedMessageData { get; }
    }
}
