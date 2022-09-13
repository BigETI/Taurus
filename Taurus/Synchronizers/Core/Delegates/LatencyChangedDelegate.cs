using System;

/// <summary>
/// Taurus synchronizers namespace
/// </summary>
namespace Taurus.Synchronizers
{
    /// <summary>
    /// Used to invoke when latency has changed
    /// </summary>
    /// <param name="oldLatency">Old latency</param>
    /// <param name="newLatency">New latency</param>
    public delegate void LatencyChangedDelegate(ILatency oldLatency, ILatency newLatency);
}
