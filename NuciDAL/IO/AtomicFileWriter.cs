using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace NuciDAL.IO
{
    internal static class AtomicFileWriter
    {
        internal static void Write(string filePath, Action<Stream> writeContent)
        {
            ArgumentNullException.ThrowIfNull(writeContent);

            string fullFilePath = GetFullFilePath(filePath);
            string temporaryFilePath = BuildTemporaryFilePath(fullFilePath);

            try
            {
                using (FileStream fileStream = OpenTemporaryStream(temporaryFilePath))
                {
                    writeContent(fileStream);
                    fileStream.Flush(true);
                }

                File.Move(temporaryFilePath, fullFilePath, true);
            }
            catch
            {
                TryDeleteTemporaryFile(temporaryFilePath);

                throw;
            }
        }

        internal static void WriteBytes(string filePath, byte[] bytes)
        {
            ArgumentNullException.ThrowIfNull(bytes);

            Write(filePath, stream => stream.Write(bytes, 0, bytes.Length));
        }

        internal static async Task WriteBytesAsync(
            string filePath,
            byte[] bytes,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(bytes);

            string fullFilePath = GetFullFilePath(filePath);
            string temporaryFilePath = BuildTemporaryFilePath(fullFilePath);

            try
            {
                using (FileStream fileStream = OpenTemporaryStream(temporaryFilePath))
                {
                    await fileStream.WriteAsync(bytes, cancellationToken);
                    await fileStream.FlushAsync(cancellationToken);
                }

                File.Move(temporaryFilePath, fullFilePath, true);
            }
            catch
            {
                TryDeleteTemporaryFile(temporaryFilePath);

                throw;
            }
        }

        private static string GetFullFilePath(string filePath)
        {
            string fullFilePath = Path.GetFullPath(filePath);

            if (string.IsNullOrEmpty(Path.GetDirectoryName(fullFilePath)))
            {
                throw new ArgumentException("File path must have a directory component.", nameof(filePath));
            }

            return fullFilePath;
        }

        private static string BuildTemporaryFilePath(string fullFilePath)
            => Path.Combine(
                Path.GetDirectoryName(fullFilePath),
                $".{Path.GetFileName(fullFilePath)}.{Guid.NewGuid():N}.tmp");

        private static FileStream OpenTemporaryStream(string temporaryFilePath)
            => new(
                temporaryFilePath,
                FileMode.CreateNew,
                FileAccess.Write,
                FileShare.None);

        private static void TryDeleteTemporaryFile(string temporaryFilePath)
        {
            try
            {
                if (File.Exists(temporaryFilePath))
                {
                    File.Delete(temporaryFilePath);
                }
            }
            catch
            {
                // Ignore cleanup failures to preserve original exception
            }
        }
    }
}