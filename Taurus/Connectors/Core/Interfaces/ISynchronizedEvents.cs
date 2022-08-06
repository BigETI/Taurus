namespace Taurus.Connectors
{
    /// <summary>
    /// An interface that represents synchronized events
    /// </summary>
    public interface ISynchronizedEvents
    {
        /// <summary>
        /// Processes all events appeared since last call
        /// </summary>
        void ProcessEvents();
    }
}
