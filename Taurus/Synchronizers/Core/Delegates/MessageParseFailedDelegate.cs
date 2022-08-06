using System;
using System.Threading.Tasks;
using Taurus.Connectors;

namespace Taurus.Synchronizers
{
    /// <summary>
    /// Used to signal a fail at parsing a message
    /// </summary>
    /// <param name="peer">Sending peer</param>
    /// <param name="expectedMessageType">Expected message type</param>
    /// <param name="bytes">Message bytes</param>
    /// <returns>Task</returns>
    public delegate Task MessageParseFailedDelegate(IPeer peer, string expectedMessageType, ReadOnlyMemory<byte> bytes);
}
