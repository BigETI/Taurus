using System;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Taurus promises
/// </summary>
namespace Taurus.Promises
{
    /// <summary>
    /// An interface that represents a promise
    /// </summary>
    /// <typeparam name="TValue">Value type</typeparam>
    public interface IPromise<TValue>
    {
        /// <summary>
        /// Is promised value set
        /// </summary>
        bool IsValueSet { get; }

        /// <summary>
        /// Promised value
        /// </summary>
        TValue Value { get; set; }

        /// <summary>
        /// Await promised value task
        /// </summary>
        Task<TValue> AwaitValueTask { get; }

        /// <summary>
        /// Awaited value
        /// </summary>
        TValue AwaitedValue { get; }

        /// <summary>
        /// Gets an awaited value task
        /// </summary>
        /// <returns>Await promised value task</returns>
        Task<TValue> GetAwaitValueTask();

        /// <summary>
        /// Gets an awaited value task
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Await promised value task</returns>
        Task<TValue> GetAwaitValueTask(CancellationToken cancellationToken);

        /// <summary>
        /// Gets an awaited promised value task
        /// </summary>
        /// <param name="timeoutTimeSpan">Timeout time span</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Await promised value task</returns>
        /// <exception cref="TimeoutException">When awaiting for value has timed out</exception>
        Task<TValue> GetAwaitValueTask(TimeSpan timeoutTimeSpan, CancellationToken cancellationToken);

        /// <summary>
        /// Gets the awaited promised value
        /// </summary>
        /// <param name="timeoutTimeSpan">Timeout time span</param>
        /// <returns>Promised value</returns>
        /// <exception cref="TimeoutException">When awaiting for value has timed out</exception>
        TValue GetAwaitedValue(TimeSpan timeoutTimeSpan);

        /// <summary>
        /// Gets the awaited promised value
        /// </summary>
        /// <param name="timeoutTimeSpan">Timeout time span</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Promised value</returns>
        /// <exception cref="TimeoutException">When awaiting for value has timed out</exception>
        TValue GetAwaitedValue(TimeSpan timeoutTimeSpan, CancellationToken cancellationToken);

        /// <summary>
        /// Tries to get promised value
        /// </summary>
        /// <param name="value">Promised value</param>
        /// <returns>"true" if promised value has been successfuly get, otherwise "false"</returns>
        bool TryGettingValue(out TValue value);
    }
}
