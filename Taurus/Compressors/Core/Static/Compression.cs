/// <summary>
/// Taurus compressors namespace
/// </summary>
namespace Taurus.Compressors
{
    /// <summary>
    /// A class used for compressing or decompressing data
    /// </summary>
    internal static class Compression
    {
        /// <summary>
        /// No compression compressor
        /// </summary>
        public static ICompressor NoCompressionCompressor { get; } = new NoCompressionCompressor();
    }
}
