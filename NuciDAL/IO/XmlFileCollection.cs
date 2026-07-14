using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace NuciDAL.IO
{
    /// <summary>
    /// XML File Collection.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="XmlFileCollection"/> class.
    /// </remarks>
    /// <param name="fileName">File name.</param>
    public class XmlFileCollection<T>(string fileName)
    {
        /// <summary>
        /// Gets the name of the file.
        /// </summary>
        /// <value>The name of the file.</value>
        public string FileName { get; } = fileName;

        /// <summary>
        /// Loads the entities.
        /// </summary>
        /// <returns>The entities.</returns>
        public IEnumerable<T> LoadEntities()
        {
            XmlSerializer serialiser = new(typeof(List<T>));

            using FileStream fileStream = new(FileName, FileMode.Open, FileAccess.Read);
            using StreamReader streamReader = new(fileStream);

            return (IEnumerable<T>)serialiser.Deserialize(streamReader);
        }

        /// <summary>
        /// Saves the entities.
        /// </summary>
        /// <param name="entities">Entities.</param>
        public void SaveEntities(IEnumerable<T> entities)
        {
            XmlSerializer serialiser = new(typeof(List<T>));

            using StringWriter stringWriter = new();

            serialiser.Serialize(stringWriter, entities);

            File.WriteAllText(FileName, stringWriter.ToString());
        }
    }
}
