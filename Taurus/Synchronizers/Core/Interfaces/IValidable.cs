/// <summary>
/// Taurus synchronizers namespace
/// </summary>
namespace Taurus.Synchronizers
{
    /// <summary>
    /// An interface that represents an object that can be validated
    /// </summary>
    public interface IValidable
    {
        /// <summary>
        /// Is object in a valid state
        /// </summary>
        bool IsValid { get; }
    }
}
