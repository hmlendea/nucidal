using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
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
        private static readonly ConcurrentDictionary<Type, PropertyInfo[]> OrderedProperties = new();

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

        private static int FirstPropertyOrder => -1;

        private static int UnorderedPropertyOrder => int.MaxValue;

        private static string IdPropertyName => "Id";

        private TDataObject ReadLine(string line)
        {
            TDataObject entity = new();
            string[] fields = line.Split(FieldSeparator);
            PropertyInfo[] properties = GetOrderedProperties(entity.GetType());

            if (fields.Length < properties.Length ||
                fields.Length > properties.Length && !string.IsNullOrWhiteSpace(fields[^1]))
            {
                throw new SerializationException(
                    $"Wrong number of CSV fields ({fields.Length}/{properties.Length})");
            }

            for (int i = 0; i < properties.Length; i++)
            {
                PropertyInfo property = properties[i];
                object value = Convert.ChangeType(fields[i], property.PropertyType, CultureInfo.InvariantCulture);
                property.SetValue(entity, value, null);
            }

            return entity;
        }

        private string BuildLine(TDataObject entity)
        {
            PropertyInfo[] properties = GetOrderedProperties(entity.GetType());
            List<string> fieldValues = new(properties.Length);

            foreach (PropertyInfo property in properties)
            {
                string fieldValue = GetPropertyFieldValue(entity, property);
                ValidateFieldValue(fieldValue, property.Name);
                fieldValues.Add(fieldValue);
            }

            return string.Join(FieldSeparator, fieldValues);
        }

        private void ValidateFieldValue(string fieldValue, string propertyName)
        {
            if (fieldValue.Contains(FieldSeparator) ||
                fieldValue.Contains('\r') ||
                fieldValue.Contains('\n'))
            {
                throw new SerializationException(
                    $"CSV field for property '{propertyName}' contains unrepresentable characters (separator, CR, or LF)");
            }
        }

        private static string GetPropertyFieldValue(TDataObject entity, PropertyInfo property)
        {
            object value = property.GetValue(entity);

            if (value is null)
            {
                return string.Empty;
            }

            return value is IFormattable formattable
                ? formattable.ToString(null, CultureInfo.InvariantCulture)
                : value.ToString();
        }

        private static PropertyInfo[] GetOrderedProperties(Type entityType)
            => OrderedProperties.GetOrAdd(entityType, BuildOrderedProperties);

        private static PropertyInfo[] BuildOrderedProperties(Type entityType)
            => [.. entityType
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Select(property => new
                {
                    Property = property,
                    Order = GetPropertyOrder(property)
                })
                .OrderBy(entry => entry.Order)
                .ThenBy(entry => entry.Property.MetadataToken)
                .Select(entry => entry.Property)];

        private static int GetPropertyOrder(PropertyInfo property)
        {
            if (property.Name == IdPropertyName)
            {
                return FirstPropertyOrder;
            }

            return property.GetCustomAttribute<CsvColumnAttribute>()?.Order ?? UnorderedPropertyOrder;
        }
    }
}
