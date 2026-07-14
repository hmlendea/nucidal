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

        static Windows1252File() => windows1252Encoding = Encoding.GetEncoding("windows-1252");

        /// <summary>
        /// Writes all text to the specified file using Windows-1252 encoding.
        /// </summary>
        /// <param name="path">The path to the file.</param>
        /// <param name="contents">The contents to write.</param>
        public static void WriteAllText(string path, string contents)
            => File.WriteAllBytes(path, windows1252Encoding.GetBytes(contents.ToCharArray()));

        /// <summary>
        /// Writes all text to the specified file using Windows-1252 encoding asynchronously.
        /// </summary>
        /// <param name="path">The path to the file.</param>
        /// <param name="contents">The contents to write.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        public static async Task WriteAllTextAsync(string path, string contents, CancellationToken cancellationToken = default)
            => await File.WriteAllBytesAsync(path, windows1252Encoding.GetBytes(contents.ToCharArray()), cancellationToken);
    }
}
