using Taurus.Validators;

/// <summary>
/// Taurus synchronizers namespace
/// </summary>
namespace Taurus.Synchronizers
{
    /// <summary>
    /// A class that describes an user authentication result
    /// </summary>
    /// <typeparam name="TUser">User type</typeparam>
    /// <typeparam name="TAuthenticationSuccessfulMessageData">Authentication successful message data type</typeparam>
    /// <typeparam name="TAuthenticationFailedMessageData">Authentication failed message data type</typeparam>
    public class UserAuthenticationResult<TUser, TAuthenticationSuccessfulMessageData, TAuthenticationFailedMessageData> :
        IUserAuthenticationResult<TUser, TAuthenticationSuccessfulMessageData, TAuthenticationFailedMessageData>
        where TUser : class, IUser
        where TAuthenticationSuccessfulMessageData : class, IBaseMessageData
        where TAuthenticationFailedMessageData : class, IBaseMessageData
    {
        /// <summary>
        /// Is user authentication successful
        /// </summary>
        public bool IsSuccessful => User != null;

        /// <summary>
        /// User
        /// </summary>
        public TUser? User { get; }

        /// <summary>
        /// Authentication successful message data
        /// </summary>
        public TAuthenticationSuccessfulMessageData? AuthenticationSuccessfulMessageData { get; }

        /// <summary>
        /// Authentication failed message data
        /// </summary>
        public TAuthenticationFailedMessageData? AuthenticationFailedMessageData { get; }

        /// <summary>
        /// Constructs a new user authentication result
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="authenticationSuccessfulMessageData">Authentication successful message data</param>
        /// <param name="authenticationFailedMessageData">Authentication failed message data</param>
        protected UserAuthenticationResult
        (
            TUser? user,
            TAuthenticationSuccessfulMessageData? authenticationSuccessfulMessageData,
            TAuthenticationFailedMessageData? authenticationFailedMessageData
        )
        {
            User = user;
            AuthenticationSuccessfulMessageData = authenticationSuccessfulMessageData;
            AuthenticationFailedMessageData = authenticationFailedMessageData;
        }

        /// <summary>
        /// Creates a new user authentication result from success
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="authenticationSuccessfulMessageData">Authentication successful message data</param>
        /// <returns>New user authentication result from success</returns>
        public static IUserAuthenticationResult<TUser, TAuthenticationSuccessfulMessageData, TAuthenticationFailedMessageData> CreateFromSuccess(TUser user, TAuthenticationSuccessfulMessageData authenticationSuccessfulMessageData)
        {
            Validator.Validate(authenticationSuccessfulMessageData, nameof(authenticationSuccessfulMessageData));
            return new UserAuthenticationResult<TUser, TAuthenticationSuccessfulMessageData, TAuthenticationFailedMessageData>(user, authenticationSuccessfulMessageData, null);
        }

        /// <summary>
        /// Creates a new user authentication result from failure
        /// </summary>
        /// <param name="authenticationFailedMessageData">Authentication failed message data</param>
        /// <returns>New user authentication result from failure</returns>
        public static IUserAuthenticationResult<TUser, TAuthenticationSuccessfulMessageData, TAuthenticationFailedMessageData> CreateFromFailure(TAuthenticationFailedMessageData authenticationFailedMessageData)
        {
            Validator.Validate(authenticationFailedMessageData, nameof(authenticationFailedMessageData));
            return new UserAuthenticationResult<TUser, TAuthenticationSuccessfulMessageData, TAuthenticationFailedMessageData>(null, null, authenticationFailedMessageData);
        }
    }
}
