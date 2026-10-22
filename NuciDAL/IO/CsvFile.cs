using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;

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
        public CsvFile(string filePath) : this(filePath, DefaultFieldSeparator) { }

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
            AtomicFileWriter.Write(FilePath, fileStream =>
            {
                using StreamWriter streamWriter = new(
                    fileStream,
                    new UTF8Encoding(false),
                    1024,
                    true);

                if (entities is not null)
                {
                    foreach (TDataObject entity in entities)
                    {
                        streamWriter.WriteLine(BuildLine(entity));
                    }
                }
            });
        }

        private static char CommentCharacter => '#';

        private static char DefaultFieldSeparator => ',';

        private TDataObject ReadLine(string line)
        {
            TDataObject entity = new();
            string[] fields = line.Split(FieldSeparator);
            PropertyInfo[] properties = GetOrderedProperties(entity.GetType());

            if (fields.Length < properties.Length ||
                fields.Length != properties.Length &&
                !string.IsNullOrWhiteSpace(fields[^1]))
            {
                throw new SerializationException(
                    $"Wrong number of CSV fields ({fields.Length}/{properties.Length})");
            }

            for (int i = 0; i < properties.Length; i++)
            {
                PropertyInfo property = properties[i];
                object value = Convert.ChangeType(fields[i], property.PropertyType);
                property.SetValue(entity, value, null);
            }

            return entity;
        }

        private string BuildLine(TDataObject entity)
        {
            PropertyInfo[] properties = GetOrderedProperties(entity.GetType());

            IEnumerable<string> fieldValues = properties
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

        private static PropertyInfo[] GetOrderedProperties(Type entityType)
        {
            PropertyInfo[] allProperties = entityType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            return allProperties
                .Select(p => new
                {
                    Property = p,
                    Order = p.Name == "Id" ? -1 : (p.GetCustomAttribute<CsvColumnAttribute>()?.Order ?? int.MaxValue)
                })
                .OrderBy(x => x.Order)
                .ThenBy(x => x.Property.MetadataToken)
                .Select(x => x.Property)
                .ToArray();
        }
    }
}
