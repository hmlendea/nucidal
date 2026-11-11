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
            string fullFilePath = Path.GetFullPath(filePath);
            string directoryPath = Path.GetDirectoryName(fullFilePath);

            if (string.IsNullOrEmpty(directoryPath))
            {
                throw new ArgumentException("File path must have a directory component.", nameof(filePath));
            }

            string temporaryFilePath = Path.Combine(
                directoryPath,
                $".{Path.GetFileName(fullFilePath)}.{Guid.NewGuid():N}.tmp");

            try
            {
                using (FileStream fileStream = new(
                    temporaryFilePath,
                    FileMode.CreateNew,
                    FileAccess.Write,
                    FileShare.None))
                {
                    writeContent(fileStream);
                    fileStream.Flush(true);
                }

                File.Move(temporaryFilePath, fullFilePath, true);
            }
            catch
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

                throw;
            }
        }

        internal static void WriteBytes(string filePath, byte[] bytes)
        {
            Write(filePath, stream => stream.Write(bytes, 0, bytes.Length));
        }

        internal static async Task WriteBytesAsync(string filePath, byte[] bytes, CancellationToken cancellationToken = default)
        {
            string fullFilePath = Path.GetFullPath(filePath);
            string directoryPath = Path.GetDirectoryName(fullFilePath);
            if (string.IsNullOrEmpty(directoryPath))
            {
                throw new ArgumentException("File path must have a directory component.", nameof(filePath));
            }

            string temporaryFilePath = Path.Combine(
                directoryPath,
                $".{Path.GetFileName(fullFilePath)}.{Guid.NewGuid():N}.tmp");

            try
            {
                using (FileStream fileStream = new(
                    temporaryFilePath,
                    FileMode.CreateNew,
                    FileAccess.Write,
                    FileShare.None))
                {
                    await fileStream.WriteAsync(bytes, cancellationToken);
                    await fileStream.FlushAsync(cancellationToken);
                }

                File.Move(temporaryFilePath, fullFilePath, true);
            }
            catch
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

                throw;
            }
        }
    }
}