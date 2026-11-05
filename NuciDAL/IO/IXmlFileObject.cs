using System;

namespace NuciDAL.IO
{
    /// <summary>
    /// XML File Object interface.
    /// </summary>
    public interface IXmlFileObject<T>
    {
        /// <summary>
        /// Gets the type.
        /// </summary>
        /// <value>The type.</value>
        Type Type { get; }

        /// <summary>
        /// Reads a <see cref="T"/> from an XML file.
        /// </summary>
        /// <param name="path">Path.</param>
        /// <returns>The deserialized object.</returns>
        T Read(string path);

        /// <summary>
        /// Writes the specified <see cref="T"/> into an XML file.
        /// </summary>
        /// <param name="path">Path.</param>
        /// <param name="obj">Object to write.</param>
        void Write(string path, T obj);
    }
}