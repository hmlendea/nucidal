using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace NuciDAL.IO
{
    /// <summary>
    /// CSV File.
    /// </summary>
    public class CsvFile<TDataObject>
        where TDataObject : new()
    {
        const char CommentCharacter = '#';

        /// <summary>
        /// Gets the name of the file.
        /// </summary>
        /// <value>The name of the file.</value>
        public string FilePath { get; private set; }

        public char FieldSeparator { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvFile"/> class.
        /// </summary>
        /// <param name="filePath">File path.</param>
        public CsvFile(string filePath)
            : this(filePath, ',')
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvFile"/> class.
        /// </summary>
        /// <param name="filePath">File path.</param>
        public CsvFile(string filePath, char fieldSeparator)
        {
            FilePath = filePath;
            FieldSeparator = fieldSeparator;
        }

        /// <summary>
        /// Loads the entities.
        /// </summary>
        /// <returns>The entities.</returns>
        public IEnumerable<TDataObject> LoadEntities()
        {
            if (!File.Exists(FilePath))
            {
                return new List<TDataObject>();
            }

            IEnumerable<TDataObject> entities = File
                .ReadAllLines(FilePath)
                .Where(line => !line.Trim().StartsWith(CommentCharacter.ToString()))
                .Select(line => ReadLine(line));

            return entities;
        }

        /// <summary>
        /// Saves the entities.
        /// </summary>
        /// <param name="entities">Entities.</param>
        public void SaveEntities(IEnumerable<TDataObject> entities)
        {
            IEnumerable<string> lines = entities.Select(entity => BuildLine(entity));
            File.WriteAllLines(FilePath, lines);
        }

        TDataObject ReadLine(string line)
        {
            TDataObject entity = new TDataObject();
            Type type = entity.GetType();
            string[] fields = line.Split(FieldSeparator);


            // TODO: This shifting is VERY HACKY and should be fixed soon
            PropertyInfo[] properties2 = type.GetProperties();
            PropertyInfo[] properties = new PropertyInfo[properties2.Length];

            Array.Copy(properties2, 0, properties, 1, properties.Length - 1);
            properties[0] = properties2[properties.Length - 1];

            if (fields.Length != properties.Length)
            {
                throw new SerializationException("Wrong number of CSV fields");
            }
            
            for (int i = 0; i < properties.Length; i++)
            {
                PropertyInfo property = properties[i];
                object value = Convert.ChangeType(fields[i], property.PropertyType);
                property.SetValue(entity, value, null);
            }

            return entity;
        }

        string BuildLine(TDataObject entity)
        {
            Type type = entity.GetType();
            PropertyInfo[] properties = type.GetProperties();
            string line = string.Empty;

            foreach (PropertyInfo property in properties)
            {
                line += property.GetValue(entity).ToString() + FieldSeparator;
            }

            return line.Substring(0, line.Length - 1);
        }
    }
}
