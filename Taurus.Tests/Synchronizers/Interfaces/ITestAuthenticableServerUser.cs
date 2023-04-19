using Taurus.Synchronizers.AuthenticableUsers;

namespace Taurus.Tests.Synchronizers
{
    internal interface ITestAuthenticableServerUser : IAuthenticableUser
    {
        public string Username { get; internal set; }
    }
}
