﻿using System;

namespace Taurus.Connectors.ENet
{
    /// <summary>
    /// An interface that represents an ENet connector
    /// </summary>
    public interface IENetConnector : IConnector
    {
        /// <summary>
        /// ENet host service timeout time in milliseconds
        /// </summary>
        uint ENetHostServiceTimeoutTime { get; }

        /// <summary>
        /// Maximal peer count
        /// </summary>
        uint MaximalPeerCount { get; }

        /// <summary>
        /// Listens to the specified port
        /// </summary>
        /// <param name="port">Port</param>
        /// <exception cref="ArgumentException">When "port" is zero</exception>
        void ListenToPort(ushort port);

        /// <summary>
        /// Stops listening to port
        /// </summary>
        void StopListening();

        /// <summary>
        /// Connects to the specified network
        /// </summary>
        /// <param name="host">Host</param>
        /// <param name="port">Port</param>
        void ConnectToENet(string host, ushort port);
    }
}
