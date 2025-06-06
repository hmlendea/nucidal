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
        public string FileName { get; private set; } = fileName;

        /// <summary>
        /// Loads the entities.
        /// </summary>
        /// <returns>The entities.</returns>
        public IEnumerable<T> LoadEntities()
        {
            XmlSerializer xs = new(typeof(List<T>));
            IEnumerable<T> entities = null;

            using (FileStream fs = new(FileName, FileMode.Open, FileAccess.Read))
            {
                using StreamReader sr = new(fs);
                entities = (IEnumerable<T>)xs.Deserialize(sr);
            }

            return entities;
        }

        /// <summary>
        /// Saves the entities.
        /// </summary>
        /// <param name="entities">Entities.</param>
        public void SaveEntities(IEnumerable<T> entities)
        {
            FileStream fs = new(FileName, FileMode.Create, FileAccess.Write);

            using StreamWriter sw = new(fs);
            XmlSerializer xs = new(typeof(List<T>));
            xs.Serialize(sw, entities);
        }
    }
}
