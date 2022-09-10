using System;
using System.Threading;
using System.Threading.Tasks;
using Taurus.Validators;

/// <summary>
/// Taurus promises namespace
/// </summary>
namespace Taurus.Promises
{
    /// <summary>
    /// A class that describes a promise
    /// </summary>
    /// <typeparam name="TValue">Value type</typeparam>
    public class Promise<TValue> : IPromise<TValue>
    {
        /// <summary>
        /// Is setting promised value unlocked
        /// </summary>
        private volatile bool isSettingValueUnlocked = true;

        /// <summary>
        /// Is promise set
        /// </summary>
        private volatile bool isValueSet = false;

        /// <summary>
        /// Promised value
        /// </summary>
        private TValue value = default!;

        /// <summary>
        /// Await value task
        /// </summary>
        private Task<TValue>? awaitValueTask;

        /// <summary>
        /// Is promised value set
        /// </summary>
        public bool IsValueSet => isValueSet;

        /// <summary>
        /// Promised value
        /// </summary>
        public TValue Value
        {
            get
            {
                if (!isValueSet)
                {
                    throw new EarlyPromisedValueAccessException();
                }
                return value;
            }
            set
            {
                if (isSettingValueUnlocked)
                {
                    isSettingValueUnlocked = false;
                    this.value = value;
                    isValueSet = true;
                }
            }
        }

        /// <summary>
        /// Await promised value task
        /// </summary>
        public Task<TValue> AwaitValueTask => GetAwaitValueTask();

        /// <summary>
        /// Awaited value
        /// </summary>
        public TValue AwaitedValue => AwaitValueTask.GetAwaiter().GetResult();

        /// <summary>
        /// Gets an awaited value task
        /// </summary>
        /// <returns>Await promised value task</returns>
        public Task<TValue> GetAwaitValueTask() => GetAwaitValueTask(CancellationToken.None);

        /// <summary>
        /// Gets an awaited value task
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Await promised value task</returns>
        public Task<TValue> GetAwaitValueTask(CancellationToken cancellationToken) =>
            awaitValueTask ??=
                isValueSet ?
                    Task.FromResult(value) :
                    Task.Factory.StartNew
                    (
                        () =>
                        {
                            while (!isValueSet)
                            {
                                // ...
                            }
                            return value;
                        },
                        cancellationToken,
                        TaskCreationOptions.LongRunning,
                        TaskScheduler.Default
                    );

        /// <summary>
        /// Gets an awaited promised value task
        /// </summary>
        /// <param name="timeoutTimeSpan">Timeout time span</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Await promised value task</returns>
        /// <exception cref="TimeoutException">When awaiting for value has timed out</exception>
        public Task<TValue> GetAwaitValueTask(TimeSpan timeoutTimeSpan, CancellationToken cancellationToken)
        {
            Validator.Validate(timeoutTimeSpan, nameof(timeoutTimeSpan), (input) => input > TimeSpan.Zero);
            return
                isValueSet ?
                    Task.FromResult(value) :
                    Task.Factory.StartNew
                    (
                        () =>
                        {
                            DateTimeOffset starting_date_time_offset = DateTimeOffset.Now;
                            while (!isValueSet)
                            {
                                if ((DateTimeOffset.Now - starting_date_time_offset) >= timeoutTimeSpan)
                                {
                                    throw new TimeoutException("Process that awaits for the promised value has timed out.");
                                }
                            }
                            return value;
                        },
                        cancellationToken,
                        TaskCreationOptions.LongRunning,
                        TaskScheduler.Default
                );
        }

        /// <summary>
        /// Gets the awaited promised value
        /// </summary>
        /// <param name="timeoutTimeSpan">Timeout time span</param>
        /// <returns>Promised value</returns>
        /// <exception cref="TimeoutException">When awaiting for value has timed out</exception>
        public TValue GetAwaitedValue(TimeSpan timeoutTimeSpan) => GetAwaitedValue(timeoutTimeSpan, CancellationToken.None);

        /// <summary>
        /// Gets the awaited promised value
        /// </summary>
        /// <param name="timeoutTimeSpan">Timeout time span</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Promised value</returns>
        /// <exception cref="TimeoutException">When awaiting for value has timed out</exception>
        public TValue GetAwaitedValue(TimeSpan timeoutTimeSpan, CancellationToken cancellationToken) => GetAwaitValueTask(timeoutTimeSpan, cancellationToken).GetAwaiter().GetResult();

        /// <summary>
        /// Tries to get promised value
        /// </summary>
        /// <param name="value">Promised value</param>
        /// <returns>"true" if promised value has been successfuly get, otherwise "false"</returns>
        public bool TryGettingValue(out TValue value)
        {
            value = this.value;
            return isValueSet;
        }
    }
}
