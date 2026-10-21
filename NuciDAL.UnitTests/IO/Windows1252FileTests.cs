using System;
using System.Diagnostics;
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
        private static string Windows1252Text => "Price: €5, “quoted” — café.";
        private static byte[] Windows1252Bytes => [0x50, 0x72, 0x69, 0x63, 0x65, 0x3A, 0x20, 0x80, 0x35, 0x2C, 0x20, 0x93, 0x71, 0x75, 0x6F, 0x74, 0x65, 0x64, 0x94, 0x20, 0x97, 0x20, 0x63, 0x61, 0x66, 0xE9, 0x2E];
        private static string IsolatedProcessEnvironmentVariable => "NUCIDAL_WINDOWS1252_ISOLATED_TEST";

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
        public void GivenFreshProcess_WhenUsingWindows1252Characters_ThenTextRoundTrips()
        {
            string projectPath = Path.GetFullPath(
                Path.Combine(AppContext.BaseDirectory, "../../../NuciDAL.UnitTests.csproj"));
            ProcessStartInfo processStartInfo = new("dotnet")
            {
                WorkingDirectory = Path.GetDirectoryName(projectPath)
            };

            processStartInfo.ArgumentList.Add("test");
            processStartInfo.ArgumentList.Add(projectPath);
            processStartInfo.ArgumentList.Add("--no-build");
            processStartInfo.ArgumentList.Add("--no-restore");
            processStartInfo.ArgumentList.Add("--filter");
            processStartInfo.ArgumentList.Add(
                "FullyQualifiedName~NuciDAL.UnitTests.IO.Windows1252FileTests.GivenWindows1252Characters_WhenWrittenSynchronouslyAndAsynchronously");
            processStartInfo.Environment[IsolatedProcessEnvironmentVariable] = "1";

            using Process process = Process.Start(processStartInfo);
            process.WaitForExit();

            Assert.That(process.ExitCode, Is.EqualTo(0));
        }

        [Test]
        public async Task GivenWindows1252Characters_WhenWrittenSynchronouslyAndAsynchronously_ThenBytesAndTextRoundTrip()
        {
            if (string.Equals(
                Environment.GetEnvironmentVariable(IsolatedProcessEnvironmentVariable),
                "1",
                StringComparison.Ordinal))
            {
                Assert.That(
                    () => Encoding.GetEncoding("windows-1252"),
                    Throws.TypeOf<ArgumentException>());
            }

            string filePath = Path.Combine(temporaryDirectoryPath, "synchronous.txt");

            Windows1252File.WriteAllText(filePath, Windows1252Text);

            Assert.That(File.ReadAllBytes(filePath), Is.EqualTo(Windows1252Bytes));
            Assert.That(
                Encoding.GetEncoding("windows-1252").GetString(File.ReadAllBytes(filePath)),
                Is.EqualTo(Windows1252Text));

            string asynchronousFilePath = Path.Combine(temporaryDirectoryPath, "asynchronous.txt");

            await Windows1252File.WriteAllTextAsync(asynchronousFilePath, Windows1252Text);

            Assert.That(File.ReadAllBytes(asynchronousFilePath), Is.EqualTo(Windows1252Bytes));
            Assert.That(
                Encoding.GetEncoding("windows-1252").GetString(File.ReadAllBytes(asynchronousFilePath)),
                Is.EqualTo(Windows1252Text));
        }

        [Test]
        public void GivenACancelledToken_WhenWritingAllTextAsynchronously_ThenTheOperationIsCancelled()
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
    }
}
