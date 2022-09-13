using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Taurus.Connectors;
using Taurus.Serializers;
using Taurus.Synchronizers.Data.Messages;
using Taurus.Validators;

/// <summary>
/// Taurus synchronizers namespace
/// </summary>
namespace Taurus.Synchronizers
{
    /// <summary>
    /// An abstract class that describes an user
    /// </summary>
    public abstract class AUser : IUser
    {
        /// <summary>
        /// Maximal measured latency time sample count
        /// </summary>
        private static readonly uint maximalMeasuredLatencyTimeSampleCount = 8U;

        /// <summary>
        /// Awaiting pong message keys
        /// </summary>
        private readonly ConcurrentDictionary<int, DateTimeOffset> awaitingPongMessageKeys = new ConcurrentDictionary<int, DateTimeOffset>();

        /// <summary>
        /// Latency
        /// </summary>
        private ILatency latency = Synchronizers.Latency.MaximalLatency;

        /// <summary>
        /// Measured latency time samples
        /// </summary>
        private List<TimeSpan> measuredLatencyTimeSamples = new List<TimeSpan>();

        /// <summary>
        /// User GUID
        /// </summary>
        public UserGUID UserGUID { get; }

        /// <summary>
        /// Peer
        /// </summary>
        public IPeer Peer { get; }

        /// <summary>
        /// Serializer
        /// </summary>
        public ISerializer Serializer { get; }

        /// <summary>
        /// Latency
        /// </summary>
        public ILatency Latency
        {
            get => latency;
            set
            {
                if (latency != value)
                {
                    ILatency old_latency = latency;
                    latency = value;
                    OnLatencyChanged?.Invoke(old_latency, latency);
                }
            }
        }

        /// <summary>
        /// Gets invoked when latency has changed
        /// </summary>
        public event LatencyChangedDelegate? OnLatencyChanged;

        /// <summary>
        /// Constructs a new user
        /// </summary>
        /// <param name="userGUID">User GUID</param>
        /// <param name="peer">Peer</param>
        /// <param name="serializer">Serializer</param>
        /// <exception cref="ArgumentException"></exception>
        protected AUser(UserGUID userGUID, IPeer peer, ISerializer serializer)
        {
            GUIDValidator.ValidateGUID(userGUID, nameof(userGUID));
            UserGUID = userGUID;
            Peer = peer;
            Serializer = serializer;
        }

        /// <summary>
        /// Sends a message to this user asynchronously
        /// </summary>
        /// <typeparam name="TMessageData">Message data type</typeparam>
        /// <param name="message">Message</param>
        /// <returns>Task</returns>
        public Task SendMessageAsync<TMessageData>(TMessageData message) where TMessageData : IBaseMessageData =>
            Peer.SendMessageAsync(Serializer.Serialize(message).ToArray());

        /// <summary>
        /// Sends an error message to this user asynchronously
        /// </summary>
        /// <param name="errorType">Error tyoe</param>
        /// <param name="errorMessage">Error message</param>
        /// <param name="errorMessageType">Error message type</param>
        /// <param name="isFatal">Is error fatal</param>
        /// <returns>Task</returns>
        public async Task SendErrorMessageAsync(EErrorType errorType, string errorMessage, string errorMessageType, bool isFatal)
        {
            Console.Error.WriteLine($"[{ errorType }] { errorMessage }");
            await SendMessageAsync(new ErrorMessageData(errorType, errorMessageType, errorMessage));
            if (isFatal)
            {
                Disconnect(EDisconnectionReason.Error);
            }
        }

        /// <summary>
        /// Sends an error message to this user asynchronously
        /// </summary>
        /// <param name="errorType">Error tyoe</param>
        /// <param name="errorMessage">Error message</param>
        /// <param name="errorMessageType">Error message type</param>
        /// <returns>Task</returns>
        public Task SendErrorMessageAsync(EErrorType errorType, string errorMessage, string errorMessageType) =>
            SendErrorMessageAsync(errorType, errorMessage, errorMessageType, false);

        /// <summary>
        /// Sends an error message to this user asynchronously
        /// </summary>
        /// <typeparam name="TMessageData">Message data type</typeparam>
        /// <param name="errorType">Error tyoe</param>
        /// <param name="errorMessage">Error message</param>
        /// <param name="isFatal">Is error fatal</param>
        /// <returns>Task</returns>
        public Task SendErrorMessageAsync<TMessageData>(EErrorType errorType, string errorMessage, bool isFatal)
            where TMessageData : IBaseMessageData =>
            SendErrorMessageAsync(errorType, errorMessage, Naming.GetMessageTypeNameFromMessageDataType<TMessageData>(), isFatal);

        /// <summary>
        /// Sends an error message to this user asynchronously
        /// </summary>
        /// <typeparam name="TMessageData">Message data type</typeparam>
        /// <param name="errorType">Error tyoe</param>
        /// <param name="errorMessage">Error message</param>
        /// <param name="isFatal">Is error fatal</param>
        /// <returns>Task</returns>
        public Task SendErrorMessageAsync<TMessageData>(EErrorType errorType, string errorMessage)
            where TMessageData : IBaseMessageData =>
            SendErrorMessageAsync<TMessageData>(errorType, errorMessage, false);

        /// <summary>
        /// Sends a ping message to this user asynchronously
        /// </summary>
        /// <returns>Task</returns>
        public Task SendPingMessageAsync()
        {
            Random random = new Random();
            int key;
            while (!awaitingPongMessageKeys.TryAdd(key = random.Next(), DateTimeOffset.Now))
            {
                // ...
            }
            return SendMessageAsync(new PingMessageData(key));
        }

        /// <summary>
        /// Receives pong message key
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>"true" if pong message key was awaited for, otherwise "false"</returns>
        public bool ReceivePongMessageKey(int key)
        {
            bool ret = awaitingPongMessageKeys.TryRemove(key, out DateTimeOffset ping_date_time_offset);
            if (ret)
            {
                TimeSpan latency_time = DateTimeOffset.Now - ping_date_time_offset;
                TimeSpan average_latency_time;
                TimeSpan measured_latency_time_samples_jitter;
                while (measuredLatencyTimeSamples.Count >= maximalMeasuredLatencyTimeSampleCount)
                {
                    measuredLatencyTimeSamples.RemoveAt(0);
                }
                measuredLatencyTimeSamples.Add(latency_time);
                if (measuredLatencyTimeSamples.Count > 1)
                {
                    TimeSpan minimal_latency_time = measuredLatencyTimeSamples[0];
                    TimeSpan maximal_latency_time = minimal_latency_time;
                    average_latency_time = minimal_latency_time;
                    for (int measured_latency_time_sample_index = 1; measured_latency_time_sample_index < measuredLatencyTimeSamples.Count; measured_latency_time_sample_index++)
                    {
                        TimeSpan measured_latency_time_sample = measuredLatencyTimeSamples[measured_latency_time_sample_index];
                        if (minimal_latency_time > measured_latency_time_sample)
                        {
                            minimal_latency_time = measured_latency_time_sample;
                        }
                        if (maximal_latency_time < measured_latency_time_sample)
                        {
                            maximal_latency_time = measured_latency_time_sample;
                        }
                        average_latency_time += measured_latency_time_sample;
                    }
                    average_latency_time /= measuredLatencyTimeSamples.Count;
                    measured_latency_time_samples_jitter = (maximal_latency_time - minimal_latency_time) * 0.5;
                }
                else
                {
                    average_latency_time = latency_time;
                    measured_latency_time_samples_jitter = latency_time;
                }
                Latency = new Latency(average_latency_time, measured_latency_time_samples_jitter);
            }
            return ret;
        }

        /// <summary>
        /// Disconnects this user
        /// </summary>
        /// <param name="disconnectionReason">Disconnection reason</param>
        public void Disconnect(EDisconnectionReason disconnectionReason) => Peer.Disconnect(disconnectionReason);
    }
}
