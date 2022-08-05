using ENet;

namespace Taurus.Connectors.ENet
{
    /// <summary>
    /// An interface that represents a connector for ENet
    /// </summary>
    public interface IENetConnector : IConnector
    {
        /// <summary>
        /// Host
        /// </summary>
        Host Host { get; }

        /// <summary>
        /// Timeout time in seconds
        /// </summary>
        uint TimeoutTime { get; }

        /// <summary>
        /// Connects to the specfied network
        /// </summary>
        /// <param name="address">ENet address</param>
        void ConnectToNetwork(Address address);
    }
}
