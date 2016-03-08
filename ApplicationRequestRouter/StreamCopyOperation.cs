using System.IO;
using System.Threading.Tasks;

namespace ApplicationRequestRouter
{
    public interface IStreamCopyOperation
    {
        Task Copy(Stream source, Stream destination);
    }

    public class StreamCopyOperation : IStreamCopyOperation
    {
        public const int BufferSize = 1024*16;

        public async Task Copy(Stream source, Stream destination)
        {
            var readBuffer = new byte[BufferSize];
            var writeBuffer = new byte[BufferSize];

            var readCount = await source.ReadAsync(readBuffer, 
                0, readBuffer.Length);

            while (readCount != 0)
            {
                var temp = readBuffer;
                readBuffer = writeBuffer;
                writeBuffer = temp;

                var writeCount = readCount;
                var writer = destination.WriteAsync(writeBuffer, 
                    0, writeCount);

                readCount = await source.ReadAsync(readBuffer, 
                    0, readBuffer.Length);

                await writer;
            }
        } 
    }
}