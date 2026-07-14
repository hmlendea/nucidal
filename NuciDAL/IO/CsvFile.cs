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
        /// <summary>
        /// Gets the name of the file.
        /// </summary>
        /// <value>The name of the file.</value>
        public string FilePath { get; } = filePath;

        /// <summary>
        /// Gets the field separator character.
        /// </summary>
        public char FieldSeparator { get; } = fieldSeparator;

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

            List<TDataObject> entities = [];

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
                throw new SerializationException(
                    $"Failed while parsing line {lineNumber}: {ex.Message}", ex);
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

        private static char CommentCharacter => '#';

        private TDataObject ReadLine(string line)
        {
            TDataObject entity = new();
            Type entityType = entity.GetType();
            string[] fields = line.Split(FieldSeparator);

            // TODO: This shifting is VERY HACKY and should be fixed soon.
            PropertyInfo[] allProperties = entityType.GetProperties();
            PropertyInfo[] reorderedProperties = new PropertyInfo[allProperties.Length];

            Array.Copy(allProperties, 0, reorderedProperties, 1, reorderedProperties.Length - 1);
            reorderedProperties[0] = allProperties[allProperties.Length - 1];

            if (fields.Length < reorderedProperties.Length ||
                fields.Length != reorderedProperties.Length &&
                !string.IsNullOrWhiteSpace(fields[fields.Length - 1]))
            {
                throw new SerializationException(
                    $"Wrong number of CSV fields ({fields.Length}/{reorderedProperties.Length})");
            }

            for (int i = 0; i < reorderedProperties.Length; i++)
            {
                PropertyInfo property = reorderedProperties[i];
                object value = Convert.ChangeType(fields[i], property.PropertyType);
                property.SetValue(entity, value, null);
            }

            return entity;
        }

        private string BuildLine(TDataObject entity)
        {
            Type entityType = entity.GetType();

            // TODO: This shifting is VERY HACKY and should be fixed soon.
            PropertyInfo[] allProperties = entityType.GetProperties();
            PropertyInfo[] reorderedProperties = new PropertyInfo[allProperties.Length];

            Array.Copy(allProperties, 0, reorderedProperties, 1, reorderedProperties.Length - 1);
            reorderedProperties[0] = allProperties[allProperties.Length - 1];

            IEnumerable<string> fieldValues = reorderedProperties
                .Select(property => GetPropertyFieldValue(entity, property));

            return string.Join(FieldSeparator, fieldValues);
        }

        private static string GetPropertyFieldValue(TDataObject entity, PropertyInfo property)
        {
            object value = property.GetValue(entity);

            if (value is null)
            {
                return string.Empty;
            }

            return value.ToString();
        }
    }
}
