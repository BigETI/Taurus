﻿using System.IO.Compression;

/// <summary>
/// Taurus compressors Gzip namespace
/// </summary>
namespace Taurus.Compressors.GZIP
{
    /// <summary>
    /// An interface that represents a Gzip compressor
    /// </summary>
    public interface IGZIPCompressor : ICompressor
    {
        /// <summary>
        /// Compression level
        /// </summary>
        CompressionLevel CompressionLevel { get; }
    }
}
