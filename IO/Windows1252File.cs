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

        public static void WriteAllText(string path, string contents)
            => File.WriteAllBytes(path, windows1252Encoding.GetBytes(contents.ToCharArray()));

        public static async Task WriteAllTextAsync(string path, string contents, CancellationToken cancellationToken = default)
            => await File.WriteAllBytesAsync(path, windows1252Encoding.GetBytes(contents.ToCharArray()), cancellationToken);
    }
}
