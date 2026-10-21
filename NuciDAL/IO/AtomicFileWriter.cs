using System;
using System.IO;

namespace NuciDAL.IO
{
    internal static class AtomicFileWriter
    {
        internal static void Write(string filePath, Action<Stream> writeContent)
        {
            string fullFilePath = Path.GetFullPath(filePath);
            string directoryPath = Path.GetDirectoryName(fullFilePath);
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
            finally
            {
                if (File.Exists(temporaryFilePath))
                {
                    File.Delete(temporaryFilePath);
                }
            }
        }
    }
}