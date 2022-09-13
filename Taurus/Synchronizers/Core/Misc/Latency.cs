using System;
using Taurus.Validators;

/// <summary>
/// Taurus synchronizers namespace
/// </summary>
namespace Taurus.Synchronizers
{
    /// <summary>
    /// A class that describes a latency
    /// </summary>
    public sealed class Latency : ILatency
    {
        /// <summary>
        /// Zero latency
        /// </summary>
        public static Latency Zero { get; } = new Latency(TimeSpan.Zero, TimeSpan.Zero);
        
        /// <summary>
        /// Maximal latency
        /// </summary>
        public static Latency MaximalLatency { get; } = new Latency(TimeSpan.MaxValue, TimeSpan.Zero);

        /// <summary>
        /// Latency time
        /// </summary>
        public TimeSpan Time { get; }

        /// <summary>
        /// Latency jitter
        /// </summary>
        public TimeSpan Jitter { get; }

        /// <summary>
        /// COnstructs a new latency
        /// </summary>
        /// <param name="time">Time</param>
        /// <param name="jitter">Jitter</param>
        public Latency(TimeSpan time, TimeSpan jitter)
        {
            Validator.Validate(time, nameof(time), (input) => input >= TimeSpan.Zero);
            Validator.Validate(jitter, nameof(jitter), (input) => input >= TimeSpan.Zero);
            Time = time;
            Jitter = jitter;
        }

        /// <summary>
        /// Gets the string representation of this object
        /// </summary>
        /// <returns>String representation of this object</returns>
        public override string ToString() => $"{ Math.Round(Time.TotalMilliseconds) } ms ± { Math.Round(Jitter.TotalMilliseconds) } ms";
    }
}
