using System.IO;
using System.Text;
using Xunit;

namespace ApplicationRequestRouter.Tests
{
    public class StreamCopyOperationFacts
    {
        [Fact]
        public async void ShouldCopySourceToDestination()
        {
            var source = new MemoryStream(new byte[] { 1, 2 });
            var destination = new MemoryStream();

            var op = new StreamCopyOperation();
            await op.Copy(source, destination);

            var copiedBytes = destination.ToArray();
            Assert.Equal(1, copiedBytes[0]);
            Assert.Equal(2, copiedBytes[1]);
        }

        [Fact]
        public async void ShouldCopyUnalignedBlocksCorrectly()
        {
            var inputBuffer = new byte[StreamCopyOperation.BufferSize + 1];
            inputBuffer[StreamCopyOperation.BufferSize] = 42;

            var source = new MemoryStream(inputBuffer);
            var destination = new MemoryStream();

            var op = new StreamCopyOperation();
            await op.Copy(source, destination);

            var copiedBytes = destination.ToArray();
            Assert.Equal(source.Length, copiedBytes.Length);
            Assert.Equal(42, copiedBytes[StreamCopyOperation.BufferSize]);
        }
    }
}