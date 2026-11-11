using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NuciDAL.IO
{
    /// <summary>
    /// File with the Windows-1252 encoding.
    /// </summary>
    public static class Windows1252File
    {
        private static readonly Encoding windows1252Encoding;

        static Windows1252File()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            windows1252Encoding = Encoding.GetEncoding(
                "windows-1252",
                EncoderFallback.ExceptionFallback,
                DecoderFallback.ExceptionFallback);
        }

        /// <summary>
        /// Writes all text to the specified file using Windows-1252 encoding.
        /// </summary>
        /// <param name="path">The path to the file.</param>
        /// <param name="contents">The contents to write.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="contents"/> is <see langword="null"/>.</exception>
        /// <exception cref="EncoderFallbackException">Thrown when <paramref name="contents"/> contains characters that cannot be encoded in Windows-1252.</exception>
        public static void WriteAllText(string path, string contents)
        {
            ArgumentNullException.ThrowIfNull(contents);

            AtomicFileWriter.WriteBytes(path, windows1252Encoding.GetBytes(contents));
        }

        /// <summary>
        /// Writes all text to the specified file using Windows-1252 encoding asynchronously.
        /// </summary>
        /// <param name="path">The path to the file.</param>
        /// <param name="contents">The contents to write.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="contents"/> is <see langword="null"/>.</exception>
        /// <exception cref="EncoderFallbackException">Thrown when <paramref name="contents"/> contains characters that cannot be encoded in Windows-1252.</exception>
        public static async Task WriteAllTextAsync(
            string path,
            string contents,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(contents);

            await AtomicFileWriter.WriteBytesAsync(path, windows1252Encoding.GetBytes(contents), cancellationToken);
        }
    }
}
