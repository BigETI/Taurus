﻿using System;
using System.IO;

namespace Taurus.Fragmenters
{
    /// <summary>
    /// A class that describes a no fragmentation fragmenter
    /// </summary>
    public sealed class NoFragmentationFragmenter : AFragmenter, INoFragmentationFragmenter
    {
        /// <summary>
        /// Fragments the specified message
        /// </summary>
        /// <param name="message">Message</param>
        /// <returns>Fragmented message</returns>
        public override ReadOnlySpan<byte> Fragment(ReadOnlySpan<byte> message) => message;

        /// <summary>
        /// Fragments the specified message
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="outputStream">Output stream</param>
        public override void Fragment(ReadOnlySpan<byte> message, Stream outputStream)
        {
            if (!outputStream.CanWrite)
            {
                throw new ArgumentException("Can not write contents to output stream.", nameof(outputStream));
            }
            outputStream.Write(message);
        }

        /// <summary>
        /// Creates a new defragmenter stream
        /// </summary>
        /// <returns>Defragmenter stream</returns>
        public override IDefragmenterStream CreateNewDefragmenterStream() => new NoDefragmentationDefragmenterStream();
    }
}
