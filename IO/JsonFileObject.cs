using System;
using System.IO;
using Newtonsoft.Json;

namespace NuciDAL.IO
{
    /// <summary>
    /// JSON File Object.
    /// </summary>
    // TODO: Create an interface
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
            T instance;

            using (StreamReader file = File.OpenText(path))
            {
                JsonSerializer serialiser = new();
                instance = (T)serialiser.Deserialize(file, Type);
            }

            return instance;
        }

        /// <summary>
        /// Writes the specified <see cref="T"/> into a JSON file.
        /// </summary>
        /// <param name="path">Path.</param>
        /// <param name="obj">Object to write.</param>
        public void Write(string path, T obj)
        {
            JsonSerializer serialiser = new();
            using StringWriter stringWriter = new();
            serialiser.Serialize(stringWriter, obj);

            File.WriteAllText(path, stringWriter.ToString());
        }
    }
}
