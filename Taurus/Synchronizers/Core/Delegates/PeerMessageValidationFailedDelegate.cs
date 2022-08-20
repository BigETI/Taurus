﻿using System;
using System.Threading.Tasks;
using Taurus.Connectors;

/// <summary>
/// Taurus synchronizers namespace
/// </summary>
namespace Taurus.Synchronizers
{
    /// <summary>
    /// Used to invoke when a peer message validation has failed
    /// </summary>
    /// <typeparam name="TMessageData">Message data tyoe</typeparam>
    /// <param name="peer">Peer</param>
    /// <param name="message">Message</param>
    /// <param name="bytes">Message bytes</param>
    /// <returns>Task</returns>
    public delegate Task PeerMessageValidationFailedDelegate<TMessageData>(IPeer peer, TMessageData message, ReadOnlyMemory<byte> bytes)
        where TMessageData : IBaseMessageData;
}