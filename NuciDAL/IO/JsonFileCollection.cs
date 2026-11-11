using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text.Json;

namespace NuciDAL.IO
{
    /// <summary>
    /// JSON File Collection.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="JsonFileCollection"/> class.
    /// </remarks>
    /// <param name="fileName">File name.</param>
    public class JsonFileCollection<T>(string fileName)
    {
        /// <summary>
        /// Gets the name of the file.
        /// </summary>
        /// <value>The name of the file.</value>
        public string FileName { get; } = fileName;

        /// <summary>
        /// Loads the entities from the JSON file.
        /// </summary>
        /// <returns>The entities.</returns>
        /// <exception cref="SerializationException">Thrown when the file contains a null payload instead of a collection.</exception>
        public IEnumerable<T> LoadEntities()
        {
            using FileStream fileStream = new(FileName, FileMode.Open, FileAccess.Read);
            using StreamReader streamReader = new(fileStream);

            IEnumerable<T>? entities = JsonSerializer.Deserialize<IEnumerable<T>>(streamReader.ReadToEnd(), options);

            if (entities is null)
            {
                throw new SerializationException($"JSON file '{FileName}' contains a null payload instead of a collection.");
            }

            return entities;
        }

        /// <summary>
        /// Saves the entities to the JSON file.
        /// </summary>
        /// <param name="entities">Entities.</param>
        public void SaveEntities(IEnumerable<T> entities)
        {
            AtomicFileWriter.Write(
                FileName,
                fileStream => JsonSerializer.Serialize(fileStream, entities, options));
        }

        private readonly JsonSerializerOptions options = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };
    }
}
