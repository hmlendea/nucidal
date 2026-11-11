using System;
using System.IO;
using System.Text;
using System.Text.Json;

namespace NuciDAL.IO
{
    /// <summary>
    /// JSON File Object.
    /// </summary>
    public class JsonFileObject<T> : IJsonFileObject<T>
    {
        /// <summary>
        /// Gets the type.
        /// </summary>
        /// <value>The type.</value>
        public Type Type { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonFileObject{T}"/> class.
        /// </summary>
        public JsonFileObject() => Type = typeof(T);

        /// <summary>
        /// Reads a <see cref="T"/> from a JSON file.
        /// </summary>
        /// <param name="path">Path.</param>
        /// <returns>The deserialized object.</returns>
        public T Read(string path)
        {
            using FileStream fileStream = new(path, FileMode.Open, FileAccess.Read);

            return JsonSerializer.Deserialize<T>(fileStream, options);
        }

        /// <summary>
        /// Writes the specified <see cref="T"/> into a JSON file.
        /// </summary>
        /// <param name="path">Path.</param>
        /// <param name="obj">Object to write.</param>
        public void Write(string path, T obj)
        {
            AtomicFileWriter.Write(path, stream =>
            {
                using StreamWriter streamWriter = new(stream, new UTF8Encoding(false), 1024, true);
                JsonSerializer.Serialize(streamWriter.BaseStream, obj, options);
            });
        }

        private readonly JsonSerializerOptions options = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
            WriteIndented = true
        };
    }
}
