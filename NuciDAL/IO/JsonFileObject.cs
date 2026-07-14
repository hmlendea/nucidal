using System;
using System.IO;
using System.Text.Json;

namespace NuciDAL.IO
{
    /// <summary>
    /// JSON File Object.
    /// </summary>
    // TODO: Create an interface.
    public class JsonFileObject<T>
    {
        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        public Type Type { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:JsonFileObject"/> class.
        /// </summary>
        public JsonFileObject() => Type = typeof(T);

        /// <summary>
        /// Reads a <see cref="T"/> from a JSON file.
        /// </summary>
        /// <param name="path">Path.</param>
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
            string json = JsonSerializer.Serialize(obj, options);

            File.WriteAllText(path, json);
        }

        private readonly JsonSerializerOptions options = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
            WriteIndented = true
        };
    }
}
