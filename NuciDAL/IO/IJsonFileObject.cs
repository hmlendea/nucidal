using System;

namespace NuciDAL.IO
{
    /// <summary>
    /// JSON File Object interface.
    /// </summary>
    public interface IJsonFileObject<T>
    {
        /// <summary>
        /// Gets the type.
        /// </summary>
        /// <value>The type.</value>
        Type Type { get; }

        /// <summary>
        /// Reads a <see cref="T"/> from a JSON file.
        /// </summary>
        /// <param name="path">Path.</param>
        /// <returns>The deserialized object.</returns>
        T Read(string path);

        /// <summary>
        /// Writes the specified <see cref="T"/> into a JSON file.
        /// </summary>
        /// <param name="path">Path.</param>
        /// <param name="obj">Object to write.</param>
        void Write(string path, T obj);
    }
}