using Taurus.Synchronizers;

namespace Taurus.Tests.Synchronizers
{
    internal interface ITestAuthenticableClientUser : IUser
    {
        public string Username { get; internal set; }
    }
}
