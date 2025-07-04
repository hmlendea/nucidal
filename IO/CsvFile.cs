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
    /// <remarks>
    /// Initializes a new instance of the <see cref="CsvFile"/> class.
    /// </remarks>
    /// <param name="filePath">File path.</param>
    public class CsvFile<TDataObject>(string filePath, char fieldSeparator) where TDataObject : new()
    {
        const char CommentCharacter = '#';

        /// <summary>
        /// Gets the name of the file.
        /// </summary>
        /// <value>The name of the file.</value>
        public string FilePath { get; private set; } = filePath;

        public char FieldSeparator { get; private set; } = fieldSeparator;

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvFile"/> class.
        /// </summary>
        /// <param name="filePath">File path.</param>
        public CsvFile(string filePath) : this(filePath, ',') { }

        /// <summary>
        /// Loads the entities.
        /// </summary>
        /// <returns>The entities.</returns>
        public IEnumerable<TDataObject> LoadEntities()
        {
            if (!File.Exists(FilePath))
            {
                return [];
            }

            IList<TDataObject> entities = [];

            int lineNumber = 0;
            try
            {
                foreach (string line in File.ReadAllLines(FilePath))
                {
                    lineNumber += 1;

                    if (line.Trim().StartsWith(CommentCharacter.ToString()))
                    {
                        continue;
                    }

                    TDataObject entity = ReadLine(line);
                    entities.Add(entity);
                }
            }
            catch (Exception ex)
            {
                throw new SerializationException($"Failed while parsing line {lineNumber}: {ex.Message}", ex);
            }

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
            TDataObject entity = new();
            Type type = entity.GetType();
            string[] fields = line.Split(FieldSeparator);

            // TODO: This shifting is VERY HACKY and should be fixed soon
            PropertyInfo[] properties2 = type.GetProperties();
            PropertyInfo[] properties = new PropertyInfo[properties2.Length];

            Array.Copy(properties2, 0, properties, 1, properties.Length - 1);
            properties[0] = properties2[properties.Length - 1];

            if (fields.Length < properties.Length ||
                fields.Length != properties.Length && !string.IsNullOrWhiteSpace(fields[fields.Length - 1]))
            {
                throw new SerializationException($"Wrong number of CSV fields ({fields.Length}/{properties.Length})");
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
            string line = string.Empty;

            // TODO: This shifting is VERY HACKY and should be fixed soon
            PropertyInfo[] properties2 = type.GetProperties();
            PropertyInfo[] properties = new PropertyInfo[properties2.Length];

            Array.Copy(properties2, 0, properties, 1, properties.Length - 1);
            properties[0] = properties2[properties.Length - 1];

            foreach (PropertyInfo property in properties)
            {
                object propertyValue = property.GetValue(entity);

                if (propertyValue is null)
                {
                    line += FieldSeparator;
                }
                else
                {
                    line += propertyValue.ToString() + FieldSeparator;
                }
            }

            return line.Substring(0, line.Length - 1);
        }
    }
}
