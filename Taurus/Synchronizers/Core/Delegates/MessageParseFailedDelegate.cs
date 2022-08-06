﻿using System;
using Taurus.Connectors;

namespace Taurus.Synchronizers
{
    /// <summary>
    /// Used to signal a fail at parsing a message
    /// </summary>
    /// <param name="peer">Sending peer</param>
    /// <param name="expectedMessageType">Expected message type</param>
    /// <param name="bson">Message BSON</param>
    public delegate void MessageParseFailedDelegate(IPeer peer, string expectedMessageType, ReadOnlySpan<byte> bson);
}