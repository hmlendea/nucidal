using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using NUnit.Framework;

using NuciDAL.IO;
using NuciDAL.IntegrationTests.Stubs;

namespace NuciDAL.IntegrationTests.IO
{
    [TestFixture]
    public sealed class Windows1252FileTests
    {
        private static string Windows1252Text => "Price: €5, “quoted” — café.";
        private static byte[] Windows1252Bytes => [0x50, 0x72, 0x69, 0x63, 0x65, 0x3A, 0x20, 0x80, 0x35, 0x2C, 0x20, 0x93, 0x71, 0x75, 0x6F, 0x74, 0x65, 0x64, 0x94, 0x20, 0x97, 0x20, 0x63, 0x61, 0x66, 0xE9, 0x2E];
        private static string UnmappableCharacter => "Ж";

        private string temporaryDirectoryPath;

        [SetUp]
        public void SetUp()
        {
            temporaryDirectoryPath = TemporaryDirectory.Create(nameof(Windows1252FileTests));
        }

        [TearDown]
        public void TearDown()
            => TemporaryDirectory.Delete(temporaryDirectoryPath);

        [Test]
        public void GivenWindows1252Characters_WhenWrittenSynchronously_ThenBytesAndTextRoundTrip()
        {
            string filePath = Path.Combine(temporaryDirectoryPath, "synchronous.txt");

            Windows1252File.WriteAllText(filePath, Windows1252Text);

            Assert.That(File.ReadAllBytes(filePath), Is.EqualTo(Windows1252Bytes));
            Assert.That(
                Encoding.GetEncoding("windows-1252").GetString(File.ReadAllBytes(filePath)),
                Is.EqualTo(Windows1252Text));
        }

        [Test]
        public async Task GivenWindows1252Characters_WhenWrittenAsynchronously_ThenBytesAndTextRoundTrip()
        {
            string filePath = Path.Combine(temporaryDirectoryPath, "asynchronous.txt");

            await Windows1252File.WriteAllTextAsync(filePath, Windows1252Text);

            Assert.That(File.ReadAllBytes(filePath), Is.EqualTo(Windows1252Bytes));
            Assert.That(
                Encoding.GetEncoding("windows-1252").GetString(File.ReadAllBytes(filePath)),
                Is.EqualTo(Windows1252Text));
        }

        [Test]
        public async Task GivenACancelledToken_WhenWritingAllTextAsynchronously_ThenTheOperationIsCancelled()
        {
            string filePath = Path.Combine(temporaryDirectoryPath, "cancelled.txt");
            using CancellationTokenSource cancellationTokenSource = new();
            cancellationTokenSource.Cancel();

            Assert.That(
                async () => await Windows1252File.WriteAllTextAsync(
                    filePath,
                    Windows1252Text,
                    cancellationTokenSource.Token),
                Throws.InstanceOf<OperationCanceledException>());
        }

        [Test]
        public async Task GivenConcurrentWrites_WhenWritingAsynchronously_ThenAllWritesComplete()
        {
            Task[] writeTasks = Enumerable.Range(0, 4)
                .Select(i =>
                {
                    string filePath = Path.Combine(temporaryDirectoryPath, $"concurrent_{i}.txt");
                    return Windows1252File.WriteAllTextAsync(filePath, Windows1252Text);
                })
                .ToArray();

            await Task.WhenAll(writeTasks);

            foreach (string filePath in Directory.GetFiles(temporaryDirectoryPath))
            {
                Assert.That(File.ReadAllBytes(filePath), Is.EqualTo(Windows1252Bytes));
            }
        }

        [Test]
        public void GivenNullContents_WhenWritingSynchronously_ThenThrowsArgumentNullException()
        {
            string filePath = Path.Combine(temporaryDirectoryPath, "null.txt");

            Assert.That(
                () => Windows1252File.WriteAllText(filePath, null!),
                Throws.InstanceOf<ArgumentNullException>()
                    .With.Property(nameof(ArgumentNullException.ParamName)).EqualTo("contents"));
        }

        [Test]
        public async Task GivenNullContents_WhenWritingAsynchronously_ThenThrowsArgumentNullException()
        {
            string filePath = Path.Combine(temporaryDirectoryPath, "null.txt");

            Assert.That(
                async () => await Windows1252File.WriteAllTextAsync(filePath, null!),
                Throws.InstanceOf<ArgumentNullException>()
                    .With.Property(nameof(ArgumentNullException.ParamName)).EqualTo("contents"));
        }

        [Test]
        public void GivenUnmappableCharacter_WhenWritingSynchronously_ThenThrowsEncoderFallbackException()
        {
            string filePath = Path.Combine(temporaryDirectoryPath, "unmappable.txt");

            Assert.That(
                () => Windows1252File.WriteAllText(filePath, UnmappableCharacter),
                Throws.InstanceOf<EncoderFallbackException>());
        }

        [Test]
        public async Task GivenUnmappableCharacter_WhenWritingAsynchronously_ThenThrowsEncoderFallbackException()
        {
            string filePath = Path.Combine(temporaryDirectoryPath, "unmappable.txt");

            Assert.That(
                async () => await Windows1252File.WriteAllTextAsync(filePath, UnmappableCharacter),
                Throws.InstanceOf<EncoderFallbackException>());
        }
    }
}