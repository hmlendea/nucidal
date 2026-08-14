using System;
using System.IO;

namespace NuciDAL.UnitTests.Stubs
{
    internal static class TemporaryDirectory
    {
        internal static string Create(string ownerName)
        {
            string path = Path.Combine(
                Path.GetTempPath(),
                ownerName,
                Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(path);

            return path;
        }

        internal static void Delete(string path)
        {
            if (!Directory.Exists(path))
            {
                return;
            }

            Directory.Delete(path, true);
        }
    }
}