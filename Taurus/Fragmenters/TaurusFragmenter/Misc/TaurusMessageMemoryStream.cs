using System.IO;

/// <summary>
/// Taurus fragmenters Taurus fragmenter namespace
/// </summary>
namespace Taurus.Fragmenters.TaurusFragmenter
{
    /// <summary>
    /// A class that describes a Taurus message memory stream
    /// </summary>
    internal class TaurusMessageMemoryStream : MemoryStream
    {
        /// <summary>
        /// Expected length of this stream
        /// </summary>
        public uint ExpectedLength { get; }

        /// <summary>
        /// Constructs a new Taurus message memory stream
        /// </summary>
        /// <param name="expectedLength">Expected length of this stream</param>
        public TaurusMessageMemoryStream(uint expectedLength) =>
            ExpectedLength = expectedLength;
    }
}

