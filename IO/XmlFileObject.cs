using System;
using System.IO;
using System.Xml.Serialization;

namespace NuciDAL.IO
{
    /// <summary>
    /// XML File Object.
    /// </summary>
    // TODO: Create an interface
    public class XmlFileObject<T>
    {
        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        public Type Type { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:XmlFileObject"/> class.
        /// </summary>
        public XmlFileObject()
        {
            Type = typeof(T);
        }

        /// <summary>
        /// Reads a <see cref="T"/> from an XML file.
        /// </summary>
        /// <param name="path">Path.</param>
        public T Read(string path)
        {
            T instance;

            using (TextReader reader = new StreamReader(path))
            {
                XmlSerializer xml = new(Type);
                instance = (T)xml.Deserialize(reader);
            }

            return instance;
        }

        /// <summary>
        /// Writes the specified <see cref="T"/> into an XML file.
        /// </summary>
        /// <param name="path">Path.</param>
        /// <param name="obj">Object to write.</param>
        public void Write(string path, T obj)
        {
            XmlSerializer serialiser = new(Type);
            using StringWriter stringWriter = new();
            serialiser.Serialize(stringWriter, obj);

            File.WriteAllText(path, stringWriter.ToString());
        }
    }
}
