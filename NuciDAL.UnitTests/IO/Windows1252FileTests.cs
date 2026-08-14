using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using NUnit.Framework;

using NuciDAL.IO;
using NuciDAL.UnitTests.Stubs;

namespace NuciDAL.UnitTests.IO
{
    [TestFixture]
    public sealed class Windows1252FileTests
    {
        private static string EncodedContent => "e=mc²";
        private static string PlainTextContent => "I use Arch btw";
        private static string FileName => "windows-1252.txt";

        private string temporaryDirectoryPath;

        [SetUp]
        public void SetUp()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            temporaryDirectoryPath = TemporaryDirectory.Create(nameof(Windows1252FileTests));
        }

        [TearDown]
        public void TearDown()
            => TemporaryDirectory.Delete(temporaryDirectoryPath);

        [Test]
        public void GivenWindows1252Content_WhenWritingAllText_ThenTheEncodedContentIsPersisted()
        {
            string filePath = Path.Combine(temporaryDirectoryPath, FileName);

            Windows1252File.WriteAllText(filePath, EncodedContent);

            byte[] persistedBytes = File.ReadAllBytes(filePath);
            string persistedContent = Encoding.GetEncoding(1252).GetString(persistedBytes);
            Assert.That(persistedContent, Is.EqualTo(EncodedContent));
        }

        [Test]
        public async Task GivenPlainTextContent_WhenWritingAllTextAsynchronously_ThenTheContentIsPersisted()
        {
            string filePath = Path.Combine(temporaryDirectoryPath, FileName);

            await Windows1252File.WriteAllTextAsync(
                filePath,
                PlainTextContent,
                CancellationToken.None);

            string persistedContent = Encoding.GetEncoding(1252).GetString(
                await File.ReadAllBytesAsync(filePath));
            Assert.That(persistedContent, Is.EqualTo(PlainTextContent));
        }

        [Test]
        public void GivenACancelledToken_WhenWritingAllTextAsynchronously_ThenTheOperationIsCancelled()
        {
            string filePath = Path.Combine(temporaryDirectoryPath, FileName);
            using CancellationTokenSource cancellationTokenSource = new();
            cancellationTokenSource.Cancel();

            Assert.That(
                async () => await Windows1252File.WriteAllTextAsync(
                    filePath,
                    PlainTextContent,
                    cancellationTokenSource.Token),
                Throws.InstanceOf<OperationCanceledException>());
        }
    }
}