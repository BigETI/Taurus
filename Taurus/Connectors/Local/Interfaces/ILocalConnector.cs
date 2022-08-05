namespace Taurus.Connectors.Local
{
    /// <summary>
    /// An interface that represents a local connector
    /// </summary>
    public interface ILocalConnector : IConnector
    {
        /// <summary>
        /// Connects to the specified target local connector
        /// </summary>
        /// <param name="targetLocalConnector">Target local connector</param>
        void ConnectToLocalConnector(ILocalConnector targetConnector);
    }
}
