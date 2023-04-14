/// <summary>
/// Taurus synchronizers authenticable users namespace
/// </summary>
namespace Taurus.Synchronizers.AuthenticableUsers
{
    /// <summary>
    /// An interface that represents an authenticated user
    /// </summary>
    public interface IAuthenticableUser : IUser
    {
        /// <summary>
        /// Is user authenticated
        /// </summary>
        bool IsAuthenticated { get; }

        /// <summary>
        /// Flags this user as authenticated
        /// </summary>
        internal void FlagAsAuthenticated();
    }
}
