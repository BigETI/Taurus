using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

/// <summary>
/// Taurus fragmenters Taurus fragmenter namespace
/// </summary>
namespace Taurus.Fragmenters.TaurusFragmenter
{
    /// <summary>
    /// A class that describes a Taurus defragmenter stream
    /// </summary>
    internal class TaurusDefragmenterStream : ADefragmenterStream, ITaurusDefragmenterStream
    {
        /// <summary>
        /// Parsed header
        /// </summary>
        private long parsedHeader;

        /// <summary>
        /// Is message length parsed
        /// </summary>
        private bool isMessageLengthParsed;

        /// <summary>
        /// Messages memory stream
        /// </summary>
        private readonly MemoryStream messagesMemoryStream = new MemoryStream();

        /// <summary>
        /// Messages binary reader
        /// </summary>
        private readonly BinaryReader messagesBinaryReader;

        /// <summary>
        /// Message memory streams
        /// </summary>
        private readonly List<TaurusMessageMemoryStream> messageMemoryStreams = new List<TaurusMessageMemoryStream>();

        /// <summary>
        /// Is message pending
        /// </summary>
        public override bool IsMessagePending
        {
            get
            {
                bool ret = parsedHeader == TaurusFragmenter.Header;
                if (!ret && (messageMemoryStreams.Count > 0))
                {
                    TaurusMessageMemoryStream message_memory_stream = messageMemoryStreams[^1];
                    ret = message_memory_stream.Length < message_memory_stream.ExpectedLength;
                }
                return ret;
            }
        }

        /// <summary>
        /// Available message count
        /// </summary>
        public override uint AvailableMessageCount => throw new NotImplementedException();

        /// <summary>
        /// Constructs a new Taurus defragmenter stream
        /// </summary>
        public TaurusDefragmenterStream() => messagesBinaryReader = new BinaryReader(messagesMemoryStream, Encoding.UTF8, true);

        /// <summary>
        /// Proceses messages
        /// </summary>
        private void ProcessMessages()
        {
            while (messagesMemoryStream.Position < messagesMemoryStream.Length)
            {
                while (parsedHeader != TaurusFragmenter.Header)
                {
                    int read_byte = messagesMemoryStream.ReadByte();
                    if (read_byte < 0)
                    {
                        break;
                    }
                    parsedHeader <<= 8;
                    parsedHeader |= (long)read_byte & 0xFF;
                }
                if (parsedHeader == TaurusFragmenter.Header)
                {
                    long remaining_messages_length = messagesMemoryStream.Length - messagesMemoryStream.Position;
                    if (isMessageLengthParsed)
                    {
                        TaurusMessageMemoryStream message_memory_stream = messageMemoryStreams[^1];
                        long read_byte_count = (message_memory_stream.ExpectedLength < remaining_messages_length) ? message_memory_stream.ExpectedLength : remaining_messages_length;
                        message_memory_stream.Write(messagesBinaryReader.ReadBytes((int)read_byte_count));
                        if (message_memory_stream.Length >= message_memory_stream.ExpectedLength)
                        {
                            parsedHeader = 0L;
                            isMessageLengthParsed = false;
                        }
                    }
                    else
                    {
                        if (remaining_messages_length >= sizeof(uint))
                        {
                            uint message_length = messagesBinaryReader.ReadUInt32();
                            TaurusMessageMemoryStream message_memory_stream = new TaurusMessageMemoryStream(message_length);
                            messageMemoryStreams.Add(message_memory_stream);
                            remaining_messages_length = messagesMemoryStream.Length - messagesMemoryStream.Position;
                            long read_byte_count = (message_length < remaining_messages_length) ? message_length : remaining_messages_length;
                            message_memory_stream.Write(messagesBinaryReader.ReadBytes((int)read_byte_count));
                            if (message_memory_stream.Length >= message_memory_stream.ExpectedLength)
                            {
                                parsedHeader = 0L;
                                isMessageLengthParsed = false;
                            }
                        }
                    }
                }
            }
            messagesMemoryStream.SetLength(0L);
        }

        /// <summary>
        /// Flushes this stream
        /// </summary>
        public override void Flush()
        {
            ProcessMessages();
            messagesMemoryStream.Flush();
        }

        /// <summary>
        /// Tries to dequeue a message
        /// </summary>
        /// <param name="message">Message</param>
        /// <returns>"true" is message has been successfully dequeued, otherwise "false"</returns>
        public override bool TryDequeuingMessage(out ReadOnlySpan<byte> message)
        {
            bool ret = false;
            message = Array.Empty<byte>();
            ProcessMessages();
            if (messageMemoryStreams.Count > 0)
            {
                TaurusMessageMemoryStream message_memory_stream = messageMemoryStreams[0];
                if (message_memory_stream.Length >= message_memory_stream.ExpectedLength)
                {
                    ret = true;
                    message_memory_stream.Seek(0L, SeekOrigin.Begin);
                    message = message_memory_stream.ToArray();
                    message = message[..(int)message_memory_stream.ExpectedLength];
                    message_memory_stream.Dispose();
                    messageMemoryStreams.RemoveAt(0);
                }
            }
            return ret;
        }

        /// <summary>
        /// Tries to dequeue a message
        /// </summary>
        /// <param name="outputStream">Output stream</param>
        /// <returns>"true" is message has been successfully dequeued, otherwise "false"</returns>
        public override bool TryDequeuingMessage(Stream outputStream)
        {
            if (!outputStream.CanWrite)
            {
                throw new ArgumentNullException("Can not write into output stream.", nameof(outputStream));
            }
            bool ret = TryDequeuingMessage(out ReadOnlySpan<byte> message);
            if (ret)
            {
                outputStream.Write(message);
            }
            return ret;
        }

        /// <summary>
        /// Writes data into this stream
        /// </summary>
        /// <param name="buffer">Buffer</param>
        /// <param name="offset">Offset</param>
        /// <param name="count">Count</param>
        public override void Write(byte[] buffer, int offset, int count)
        {
            messagesMemoryStream.Write(buffer, offset, count);
            ProcessMessages();
        }

        /// <summary>
        /// Disposes this object
        /// </summary>
        /// <param name="disposing">Is disposing this object</param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                messagesMemoryStream.Dispose();
                foreach (TaurusMessageMemoryStream message_memory_stream in messageMemoryStreams)
                {
                    message_memory_stream.Dispose();
                }
                messageMemoryStreams.Clear();
            }
        }
    }
}
