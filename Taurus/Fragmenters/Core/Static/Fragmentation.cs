/// <summary>
/// Taurus fragmenters namespace
/// </summary>
namespace Taurus.Fragmenters
{
    /// <summary>
    /// A class that contains functionalities for fragmentation
    /// </summary>
    public static class Fragmentation
    {
        /// <summary>
        /// No fragmentation fragmenter
        /// </summary>
        public static IFragmenter NoFragmentationFragmenter { get; } = new NoFragmentationFragmenter();
    }
}
