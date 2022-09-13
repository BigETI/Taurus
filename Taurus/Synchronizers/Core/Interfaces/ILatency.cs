using System;

/// <summary>
/// TAurus synchronizers namespace
/// </summary>
namespace Taurus.Synchronizers
{
    /// <summary>
    /// An interface that represents a latency
    /// </summary>
    public interface ILatency
    {
        /// <summary>
        /// Latency time
        /// </summary>
        public TimeSpan Time { get; }

        /// <summary>
        /// Latency jitter
        /// </summary>
        public TimeSpan Jitter { get; }
    }
}
