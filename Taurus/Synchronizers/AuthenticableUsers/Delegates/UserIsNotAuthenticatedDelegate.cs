using System.Threading.Tasks;

/// <summary>
/// Taurus synchronizers authenticable users namespace
/// </summary>
namespace Taurus.Synchronizers.AuthenticableUsers
{
    /// <summary>
    /// Used to invoke when peer is not authenticated
    /// </summary>
    /// <typeparam name="TAuthenticableUser">Authenticable user type</typeparam>
    /// <param name="authenticableUser">Authenticable user</param>
    /// <returns>Task</returns>
    public delegate Task UserIsNotAuthenticatedDelegate<TAuthenticableUser>(TAuthenticableUser authenticableUser)
        where TAuthenticableUser : IAuthenticableUser;
}
