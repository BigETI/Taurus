/// <summary>
/// Taurus namespace
/// </summary>
namespace Taurus
{
    /// <summary>
    /// An interface that represents processable events
    /// </summary>
    public interface IProcessableEvents
    {
        /// <summary>
        /// Processes all events appeared since last call
        /// </summary>
        void ProcessEvents();
    }
}
