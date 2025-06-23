using System.Collections.Generic;
using System.IO;
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
        readonly JsonSerializerOptions options = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        /// <summary>
        /// Gets the name of the file.
        /// </summary>
        /// <value>The name of the file.</value>
        public string FileName { get; private set; } = fileName;

        /// <summary>
        /// Loads the entities from the JSON file.
        /// </summary>
        /// <returns>The entities.</returns>
        public IEnumerable<T> LoadEntities()
        {
            IEnumerable<T> entities = null;

            using (FileStream fs = new(FileName, FileMode.Open, FileAccess.Read))
            {
                using StreamReader sr = new(fs);
                string json = sr.ReadToEnd();

                entities = JsonSerializer.Deserialize<IEnumerable<T>>(json, options);
            }

            return entities;
        }

        /// <summary>
        /// Saves the entities to the JSON file.
        /// </summary>
        /// <param name="entities">Entities.</param>
        public void SaveEntities(IEnumerable<T> entities)
        {
            string json = JsonSerializer.Serialize(entities, options);

            using FileStream fs = new(FileName, FileMode.Create, FileAccess.Write);
            using StreamWriter sw = new(fs);

            sw.Write(json);
        }
    }
}
