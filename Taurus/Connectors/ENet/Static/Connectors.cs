﻿using ENet;
using Taurus.Compressors;
using Taurus.Connectors.ENet;
using Taurus.Fragmenters;

/// <summary>
/// Taurus connectors namespace
/// </summary>
namespace Taurus.Connectors
{
    /// <summary>
    /// A class that provides functionalities to create connectors
    /// </summary>
    public static partial class Connectors
    {
        /// <summary>
        /// Creates a new ENet connector internally
        /// </summary>
        /// <param name="host">ENet Host</param>
        /// <param name="timeoutTime">Timeout time</param>
        /// <param name="onHandlePeerConnectionAttempt">Gets invoked when a peer conection attempt needs to be handled</param>
        /// <param name="fragmenter">Fragmenter</param>
        /// <param name="compressor">Compressor</param>
        /// <returns>New ENet connector</returns>
        private static IENetConnector CreateNewENetConnectorInternal
        (
            Host host,
            uint timeoutTime,
            HandlePeerConnectionAttemptDelegate onHandlePeerConnectionAttempt,
            IFragmenter? fragmenter,
            ICompressor? compressor
        ) => new ENetConnector(host, timeoutTime, onHandlePeerConnectionAttempt, fragmenter, compressor);
        
        /// <summary>
        /// Creates a new ENet connector
        /// </summary>
        /// <param name="host">ENet Host</param>
        /// <param name="timeoutTime">Timeout time</param>
        /// <param name="onHandlePeerConnectionAttempt">Gets invoked when a peer conection attempt needs to be handled</param>
        /// <returns>New ENet connector</returns>
        public static IENetConnector CreateNewENetConnector
        (
            Host host,
            uint timeoutTime,
            HandlePeerConnectionAttemptDelegate onHandlePeerConnectionAttempt
        ) => CreateNewENetConnectorInternal(host, timeoutTime, onHandlePeerConnectionAttempt, null, null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="host">ENet Host</param>
        /// <param name="timeoutTime">Timeout time</param>
        /// <param name="onHandlePeerConnectionAttempt"></param>
        /// <param name="fragmenter">Fragmenter</param>
        /// <returns>New ENet connector</returns>
        public static IENetConnector CreateNewENetConnector
        (
            Host host,
            uint timeoutTime,
            HandlePeerConnectionAttemptDelegate onHandlePeerConnectionAttempt,
            IFragmenter fragmenter
        ) => CreateNewENetConnectorInternal(host, timeoutTime, onHandlePeerConnectionAttempt, fragmenter, null);

        /// <summary>
        /// Creates a new ENet connector
        /// </summary>
        /// <param name="host">ENet Host</param>
        /// <param name="timeoutTime">Timeout time</param>
        /// <param name="onHandlePeerConnectionAttempt">Gets invoked when a peer conection attempt needs to be handled</param>
        /// <param name="compressor">Compressor</param>
        /// <returns>New ENet connector</returns>
        public static IENetConnector CreateNewENetConnector
        (
            Host host,
            uint timeoutTime,
            HandlePeerConnectionAttemptDelegate onHandlePeerConnectionAttempt,
            ICompressor? compressor
        ) => CreateNewENetConnectorInternal(host, timeoutTime, onHandlePeerConnectionAttempt, null, compressor);

        /// <summary>
        /// Creates a new ENet connector
        /// </summary>
        /// <param name="host">ENet Host</param>
        /// <param name="timeoutTime">Timeout time</param>
        /// <param name="onHandlePeerConnectionAttempt">Gets invoked when a peer conection attempt needs to be handled</param>
        /// <param name="fragmenter">Fragmenter</param>
        /// <param name="compressor">Compressor</param>
        /// <returns>New ENet connector</returns>
        public static IENetConnector CreateNewENetConnector
        (
            Host host,
            uint timeoutTime,
            HandlePeerConnectionAttemptDelegate onHandlePeerConnectionAttempt,
            IFragmenter fragmenter,
            ICompressor compressor
        ) => CreateNewENetConnectorInternal(host, timeoutTime, onHandlePeerConnectionAttempt, fragmenter, compressor);
    }
}
