using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Taurus.Fragmenters.TaurusFragmenter
{
    internal class TaurusDefragmenterStream : ADefragmenterStream, ITaurusDefragmenterStream
    {
        private long parsedHeader;

        private bool isMessageLengthParsed;

        private readonly MemoryStream messagesMemoryStream = new MemoryStream();

        private readonly BinaryReader messagesBinaryReader;

        private readonly List<TaurusMessageMemoryStream> messageMemoryStreams = new List<TaurusMessageMemoryStream>();

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

        public override uint AvailableMessageCount => throw new NotImplementedException();

        public TaurusDefragmenterStream() => messagesBinaryReader = new BinaryReader(messagesMemoryStream, Encoding.UTF8, true);

        public void ProcessMessages()
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
                        TaurusMessageMemoryStream message_memory_stream = messageMemoryStreams[messageMemoryStreams.Count - 1];
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

        public override void Flush()
        {
            ProcessMessages();
            messagesMemoryStream.Flush();
        }

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

        public override void Write(byte[] buffer, int offset, int count)
        {
            messagesMemoryStream.Write(buffer, offset, count);
            ProcessMessages();
        }

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
