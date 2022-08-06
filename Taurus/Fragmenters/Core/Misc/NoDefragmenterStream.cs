using System;
using System.IO;

namespace Taurus.Fragmenters
{
    internal class NoDefragmenterStream : ADefragmenterStream, INoDefragmenterStream
    {
        private readonly MemoryStream memoryStream = new MemoryStream();

        public override bool IsMessagePending { get; } = false;

        public override uint AvailableMessageCount { get; } = 0U;

        public override void Flush() => memoryStream.Flush();

        public override void Write(byte[] buffer, int offset, int count) => memoryStream.Write(buffer, offset, count);

        public override bool TryDequeuingMessage(out ReadOnlySpan<byte> message)
        {
            bool ret = memoryStream.Length > 0;
            if (ret)
            {
                memoryStream.Seek(0L, SeekOrigin.Begin);
                message = memoryStream.ToArray();
            }
            else
            {
                message = Array.Empty<byte>();
            }
            return ret;
        }

        public override bool TryDequeuingMessage(Stream outputStream)
        {
            if (!outputStream.CanWrite)
            {
                throw new ArgumentException("Can not write contents to output stream.", nameof(outputStream));
            }
            bool ret = memoryStream.Length > 0L;
            if (ret)
            {
                memoryStream.CopyTo(outputStream);
                memoryStream.SetLength(0L);
            }
            return ret;
        }
    }
}
